
(function(l, r) { if (l.getElementById('livereloadscript')) return; r = l.createElement('script'); r.async = 1; r.src = '//' + (window.location.host || 'localhost').split(':')[0] + ':35729/livereload.js?snipver=1'; r.id = 'livereloadscript'; l.head.appendChild(r) })(window.document);
var app = (function () {
    'use strict';

    function noop() { }
    function add_location(element, file, line, column, char) {
        element.__svelte_meta = {
            loc: { file, line, column, char }
        };
    }
    function run(fn) {
        return fn();
    }
    function blank_object() {
        return Object.create(null);
    }
    function run_all(fns) {
        fns.forEach(run);
    }
    function is_function(thing) {
        return typeof thing === 'function';
    }
    function safe_not_equal(a, b) {
        return a != a ? b == b : a !== b || ((a && typeof a === 'object') || typeof a === 'function');
    }

    function append(target, node) {
        target.appendChild(node);
    }
    function insert(target, node, anchor) {
        target.insertBefore(node, anchor || null);
    }
    function detach(node) {
        node.parentNode.removeChild(node);
    }
    function destroy_each(iterations, detaching) {
        for (let i = 0; i < iterations.length; i += 1) {
            if (iterations[i])
                iterations[i].d(detaching);
        }
    }
    function element(name) {
        return document.createElement(name);
    }
    function text(data) {
        return document.createTextNode(data);
    }
    function space() {
        return text(' ');
    }
    function empty() {
        return text('');
    }
    function listen(node, event, handler, options) {
        node.addEventListener(event, handler, options);
        return () => node.removeEventListener(event, handler, options);
    }
    function attr(node, attribute, value) {
        if (value == null)
            node.removeAttribute(attribute);
        else if (node.getAttribute(attribute) !== value)
            node.setAttribute(attribute, value);
    }
    function children(element) {
        return Array.from(element.childNodes);
    }
    function set_style(node, key, value, important) {
        node.style.setProperty(key, value, important ? 'important' : '');
    }
    function custom_event(type, detail) {
        const e = document.createEvent('CustomEvent');
        e.initCustomEvent(type, false, false, detail);
        return e;
    }

    let current_component;
    function set_current_component(component) {
        current_component = component;
    }

    const dirty_components = [];
    const binding_callbacks = [];
    const render_callbacks = [];
    const flush_callbacks = [];
    const resolved_promise = Promise.resolve();
    let update_scheduled = false;
    function schedule_update() {
        if (!update_scheduled) {
            update_scheduled = true;
            resolved_promise.then(flush);
        }
    }
    function add_render_callback(fn) {
        render_callbacks.push(fn);
    }
    function flush() {
        const seen_callbacks = new Set();
        do {
            // first, call beforeUpdate functions
            // and update components
            while (dirty_components.length) {
                const component = dirty_components.shift();
                set_current_component(component);
                update(component.$$);
            }
            while (binding_callbacks.length)
                binding_callbacks.pop()();
            // then, once components are updated, call
            // afterUpdate functions. This may cause
            // subsequent updates...
            for (let i = 0; i < render_callbacks.length; i += 1) {
                const callback = render_callbacks[i];
                if (!seen_callbacks.has(callback)) {
                    callback();
                    // ...so guard against infinite loops
                    seen_callbacks.add(callback);
                }
            }
            render_callbacks.length = 0;
        } while (dirty_components.length);
        while (flush_callbacks.length) {
            flush_callbacks.pop()();
        }
        update_scheduled = false;
    }
    function update($$) {
        if ($$.fragment !== null) {
            $$.update($$.dirty);
            run_all($$.before_update);
            $$.fragment && $$.fragment.p($$.dirty, $$.ctx);
            $$.dirty = null;
            $$.after_update.forEach(add_render_callback);
        }
    }
    const outroing = new Set();
    let outros;
    function group_outros() {
        outros = {
            r: 0,
            c: [],
            p: outros // parent group
        };
    }
    function check_outros() {
        if (!outros.r) {
            run_all(outros.c);
        }
        outros = outros.p;
    }
    function transition_in(block, local) {
        if (block && block.i) {
            outroing.delete(block);
            block.i(local);
        }
    }
    function transition_out(block, local, detach, callback) {
        if (block && block.o) {
            if (outroing.has(block))
                return;
            outroing.add(block);
            outros.c.push(() => {
                outroing.delete(block);
                if (callback) {
                    if (detach)
                        block.d(1);
                    callback();
                }
            });
            block.o(local);
        }
    }
    function create_component(block) {
        block && block.c();
    }
    function mount_component(component, target, anchor) {
        const { fragment, on_mount, on_destroy, after_update } = component.$$;
        fragment && fragment.m(target, anchor);
        // onMount happens before the initial afterUpdate
        add_render_callback(() => {
            const new_on_destroy = on_mount.map(run).filter(is_function);
            if (on_destroy) {
                on_destroy.push(...new_on_destroy);
            }
            else {
                // Edge case - component was destroyed immediately,
                // most likely as a result of a binding initialising
                run_all(new_on_destroy);
            }
            component.$$.on_mount = [];
        });
        after_update.forEach(add_render_callback);
    }
    function destroy_component(component, detaching) {
        const $$ = component.$$;
        if ($$.fragment !== null) {
            run_all($$.on_destroy);
            $$.fragment && $$.fragment.d(detaching);
            // TODO null out other refs, including component.$$ (but need to
            // preserve final state?)
            $$.on_destroy = $$.fragment = null;
            $$.ctx = {};
        }
    }
    function make_dirty(component, key) {
        if (!component.$$.dirty) {
            dirty_components.push(component);
            schedule_update();
            component.$$.dirty = blank_object();
        }
        component.$$.dirty[key] = true;
    }
    function init(component, options, instance, create_fragment, not_equal, props) {
        const parent_component = current_component;
        set_current_component(component);
        const prop_values = options.props || {};
        const $$ = component.$$ = {
            fragment: null,
            ctx: null,
            // state
            props,
            update: noop,
            not_equal,
            bound: blank_object(),
            // lifecycle
            on_mount: [],
            on_destroy: [],
            before_update: [],
            after_update: [],
            context: new Map(parent_component ? parent_component.$$.context : []),
            // everything else
            callbacks: blank_object(),
            dirty: null
        };
        let ready = false;
        $$.ctx = instance
            ? instance(component, prop_values, (key, ret, value = ret) => {
                if ($$.ctx && not_equal($$.ctx[key], $$.ctx[key] = value)) {
                    if ($$.bound[key])
                        $$.bound[key](value);
                    if (ready)
                        make_dirty(component, key);
                }
                return ret;
            })
            : prop_values;
        $$.update();
        ready = true;
        run_all($$.before_update);
        // `false` as a special case of no DOM component
        $$.fragment = create_fragment ? create_fragment($$.ctx) : false;
        if (options.target) {
            if (options.hydrate) {
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.l(children(options.target));
            }
            else {
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.c();
            }
            if (options.intro)
                transition_in(component.$$.fragment);
            mount_component(component, options.target, options.anchor);
            flush();
        }
        set_current_component(parent_component);
    }
    class SvelteComponent {
        $destroy() {
            destroy_component(this, 1);
            this.$destroy = noop;
        }
        $on(type, callback) {
            const callbacks = (this.$$.callbacks[type] || (this.$$.callbacks[type] = []));
            callbacks.push(callback);
            return () => {
                const index = callbacks.indexOf(callback);
                if (index !== -1)
                    callbacks.splice(index, 1);
            };
        }
        $set() {
            // overridden by instance, if it has props
        }
    }

    function dispatch_dev(type, detail) {
        document.dispatchEvent(custom_event(type, detail));
    }
    function append_dev(target, node) {
        dispatch_dev("SvelteDOMInsert", { target, node });
        append(target, node);
    }
    function insert_dev(target, node, anchor) {
        dispatch_dev("SvelteDOMInsert", { target, node, anchor });
        insert(target, node, anchor);
    }
    function detach_dev(node) {
        dispatch_dev("SvelteDOMRemove", { node });
        detach(node);
    }
    function listen_dev(node, event, handler, options, has_prevent_default, has_stop_propagation) {
        const modifiers = options === true ? ["capture"] : options ? Array.from(Object.keys(options)) : [];
        if (has_prevent_default)
            modifiers.push('preventDefault');
        if (has_stop_propagation)
            modifiers.push('stopPropagation');
        dispatch_dev("SvelteDOMAddEventListener", { node, event, handler, modifiers });
        const dispose = listen(node, event, handler, options);
        return () => {
            dispatch_dev("SvelteDOMRemoveEventListener", { node, event, handler, modifiers });
            dispose();
        };
    }
    function attr_dev(node, attribute, value) {
        attr(node, attribute, value);
        if (value == null)
            dispatch_dev("SvelteDOMRemoveAttribute", { node, attribute });
        else
            dispatch_dev("SvelteDOMSetAttribute", { node, attribute, value });
    }
    function prop_dev(node, property, value) {
        node[property] = value;
        dispatch_dev("SvelteDOMSetProperty", { node, property, value });
    }
    function set_data_dev(text, data) {
        data = '' + data;
        if (text.data === data)
            return;
        dispatch_dev("SvelteDOMSetData", { node: text, data });
        text.data = data;
    }
    class SvelteComponentDev extends SvelteComponent {
        constructor(options) {
            if (!options || (!options.target && !options.$$inline)) {
                throw new Error(`'target' is a required option`);
            }
            super();
        }
        $destroy() {
            super.$destroy();
            this.$destroy = () => {
                console.warn(`Component was already destroyed`); // eslint-disable-line no-console
            };
        }
    }

    /* src/SearchResult.svelte generated by Svelte v3.15.0 */

    const file = "src/SearchResult.svelte";

    // (41:2) {#if !descriptor.parent && descriptor.type}
    function create_if_block(ctx) {
    	let br;
    	let t0;
    	let a;
    	let t1;
    	let a_alt_value;
    	let a_href_value;

    	const block = {
    		c: function create() {
    			br = element("br");
    			t0 = space();
    			a = element("a");
    			t1 = text("Show me the data!");
    			add_location(br, file, 41, 4, 915);
    			attr_dev(a, "alt", a_alt_value = ctx.descriptor.name);
    			attr_dev(a, "href", a_href_value = `/api/data/${ctx.descriptor.module}/${ctx.descriptor.name}`);
    			add_location(a, file, 42, 4, 926);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, br, anchor);
    			insert_dev(target, t0, anchor);
    			insert_dev(target, a, anchor);
    			append_dev(a, t1);
    		},
    		p: function update(changed, ctx) {
    			if (changed.descriptor && a_alt_value !== (a_alt_value = ctx.descriptor.name)) {
    				attr_dev(a, "alt", a_alt_value);
    			}

    			if (changed.descriptor && a_href_value !== (a_href_value = `/api/data/${ctx.descriptor.module}/${ctx.descriptor.name}`)) {
    				attr_dev(a, "href", a_href_value);
    			}
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(br);
    			if (detaching) detach_dev(t0);
    			if (detaching) detach_dev(a);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block.name,
    		type: "if",
    		source: "(41:2) {#if !descriptor.parent && descriptor.type}",
    		ctx
    	});

    	return block;
    }

    function create_fragment(ctx) {
    	let div;
    	let h2;
    	let t0_value = ctx.descriptor.module + "";
    	let t0;
    	let t1;
    	let t2_value = (ctx.descriptor.parent ? ctx.descriptor.parent + "." : "") + "";
    	let t2;
    	let t3_value = ctx.descriptor.name + "";
    	let t3;
    	let t4;
    	let p;
    	let t5_value = (ctx.descriptor.description || "No Description") + "";
    	let t5;
    	let t6;
    	let a;
    	let t7;
    	let t8_value = ctx.descriptor.module + "";
    	let t8;
    	let a_alt_value;
    	let a_href_value;
    	let t9;
    	let dispose;
    	let if_block = !ctx.descriptor.parent && ctx.descriptor.type && create_if_block(ctx);

    	const block = {
    		c: function create() {
    			div = element("div");
    			h2 = element("h2");
    			t0 = text(t0_value);
    			t1 = text(" - ");
    			t2 = text(t2_value);
    			t3 = text(t3_value);
    			t4 = space();
    			p = element("p");
    			t5 = text(t5_value);
    			t6 = space();
    			a = element("a");
    			t7 = text("Module: ");
    			t8 = text(t8_value);
    			t9 = space();
    			if (if_block) if_block.c();
    			attr_dev(h2, "class", "svelte-8inx22");
    			add_location(h2, file, 33, 2, 573);
    			attr_dev(p, "class", "description svelte-8inx22");
    			add_location(p, file, 36, 2, 682);
    			attr_dev(a, "alt", a_alt_value = ctx.descriptor.module);
    			attr_dev(a, "href", a_href_value = `/${ctx.descriptor.module}/index.html`);
    			add_location(a, file, 37, 2, 756);
    			attr_dev(div, "class", "descriptor svelte-8inx22");
    			add_location(div, file, 32, 0, 506);
    			dispose = listen_dev(div, "click", ctx.click_handler, false, false, false);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, h2);
    			append_dev(h2, t0);
    			append_dev(h2, t1);
    			append_dev(h2, t2);
    			append_dev(h2, t3);
    			append_dev(div, t4);
    			append_dev(div, p);
    			append_dev(p, t5);
    			append_dev(div, t6);
    			append_dev(div, a);
    			append_dev(a, t7);
    			append_dev(a, t8);
    			append_dev(div, t9);
    			if (if_block) if_block.m(div, null);
    		},
    		p: function update(changed, ctx) {
    			if (changed.descriptor && t0_value !== (t0_value = ctx.descriptor.module + "")) set_data_dev(t0, t0_value);
    			if (changed.descriptor && t2_value !== (t2_value = (ctx.descriptor.parent ? ctx.descriptor.parent + "." : "") + "")) set_data_dev(t2, t2_value);
    			if (changed.descriptor && t3_value !== (t3_value = ctx.descriptor.name + "")) set_data_dev(t3, t3_value);
    			if (changed.descriptor && t5_value !== (t5_value = (ctx.descriptor.description || "No Description") + "")) set_data_dev(t5, t5_value);
    			if (changed.descriptor && t8_value !== (t8_value = ctx.descriptor.module + "")) set_data_dev(t8, t8_value);

    			if (changed.descriptor && a_alt_value !== (a_alt_value = ctx.descriptor.module)) {
    				attr_dev(a, "alt", a_alt_value);
    			}

    			if (changed.descriptor && a_href_value !== (a_href_value = `/${ctx.descriptor.module}/index.html`)) {
    				attr_dev(a, "href", a_href_value);
    			}

    			if (!ctx.descriptor.parent && ctx.descriptor.type) {
    				if (if_block) {
    					if_block.p(changed, ctx);
    				} else {
    					if_block = create_if_block(ctx);
    					if_block.c();
    					if_block.m(div, null);
    				}
    			} else if (if_block) {
    				if_block.d(1);
    				if_block = null;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    			if (if_block) if_block.d();
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance($$self, $$props, $$invalidate) {
    	let { descriptor } = $$props;

    	let selectNode = descriptor => {
    		
    	};

    	const writable_props = ["descriptor"];

    	Object.keys($$props).forEach(key => {
    		if (!~writable_props.indexOf(key) && key.slice(0, 2) !== "$$") console.warn(`<SearchResult> was created with unknown prop '${key}'`);
    	});

    	const click_handler = () => selectNode(descriptor);

    	$$self.$set = $$props => {
    		if ("descriptor" in $$props) $$invalidate("descriptor", descriptor = $$props.descriptor);
    	};

    	$$self.$capture_state = () => {
    		return { descriptor, selectNode };
    	};

    	$$self.$inject_state = $$props => {
    		if ("descriptor" in $$props) $$invalidate("descriptor", descriptor = $$props.descriptor);
    		if ("selectNode" in $$props) $$invalidate("selectNode", selectNode = $$props.selectNode);
    	};

    	return { descriptor, selectNode, click_handler };
    }

    class SearchResult extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance, create_fragment, safe_not_equal, { descriptor: 0 });

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "SearchResult",
    			options,
    			id: create_fragment.name
    		});

    		const { ctx } = this.$$;
    		const props = options.props || ({});

    		if (ctx.descriptor === undefined && !("descriptor" in props)) {
    			console.warn("<SearchResult> was created without expected prop 'descriptor'");
    		}
    	}

    	get descriptor() {
    		throw new Error("<SearchResult>: Props cannot be read directly from the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}

    	set descriptor(value) {
    		throw new Error("<SearchResult>: Props cannot be set directly on the component instance unless compiling with 'accessors: true' or '<svelte:options accessors/>'");
    	}
    }

    /* src/Main.svelte generated by Svelte v3.15.0 */
    const file$1 = "src/Main.svelte";

    function get_each_context(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.d = list[i];
    	return child_ctx;
    }

    // (32:0) {#each data as d}
    function create_each_block(ctx) {
    	let current;

    	const searchresult = new SearchResult({
    			props: { descriptor: ctx.d },
    			$$inline: true
    		});

    	const block = {
    		c: function create() {
    			create_component(searchresult.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(searchresult, target, anchor);
    			current = true;
    		},
    		p: function update(changed, ctx) {
    			const searchresult_changes = {};
    			if (changed.data) searchresult_changes.descriptor = ctx.d;
    			searchresult.$set(searchresult_changes);
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(searchresult.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(searchresult.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(searchresult, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block.name,
    		type: "each",
    		source: "(32:0) {#each data as d}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$1(ctx) {
    	let h1;
    	let t1;
    	let p;
    	let t2;
    	let a;
    	let t4;
    	let t5;
    	let div;
    	let h2;
    	let t7;
    	let input;
    	let t8;
    	let each_1_anchor;
    	let current;
    	let dispose;
    	let each_value = ctx.data;
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
    	}

    	const out = i => transition_out(each_blocks[i], 1, 1, () => {
    		each_blocks[i] = null;
    	});

    	const block = {
    		c: function create() {
    			h1 = element("h1");
    			h1.textContent = "Welcome to ZDragon!";
    			t1 = space();
    			p = element("p");
    			t2 = text("Visit\n  ");
    			a = element("a");
    			a.textContent = "ZDragon.nl";
    			t4 = text("\n  to learn more about zdragon!");
    			t5 = space();
    			div = element("div");
    			h2 = element("h2");
    			h2.textContent = "Search your models:";
    			t7 = space();
    			input = element("input");
    			t8 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			each_1_anchor = empty();
    			attr_dev(h1, "class", "title");
    			add_location(h1, file$1, 19, 0, 366);
    			attr_dev(a, "href", "https://zdragon.nl");
    			add_location(a, file$1, 22, 2, 423);
    			add_location(p, file$1, 20, 0, 409);
    			add_location(h2, file$1, 27, 2, 512);
    			attr_dev(input, "type", "text");
    			add_location(input, file$1, 28, 2, 543);
    			add_location(div, file$1, 26, 0, 504);
    			dispose = listen_dev(input, "change", ctx.change_handler, false, false, false);
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h1, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, p, anchor);
    			append_dev(p, t2);
    			append_dev(p, a);
    			append_dev(p, t4);
    			insert_dev(target, t5, anchor);
    			insert_dev(target, div, anchor);
    			append_dev(div, h2);
    			append_dev(div, t7);
    			append_dev(div, input);
    			insert_dev(target, t8, anchor);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(target, anchor);
    			}

    			insert_dev(target, each_1_anchor, anchor);
    			current = true;
    		},
    		p: function update(changed, ctx) {
    			if (changed.data) {
    				each_value = ctx.data;
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(changed, child_ctx);
    						transition_in(each_blocks[i], 1);
    					} else {
    						each_blocks[i] = create_each_block(child_ctx);
    						each_blocks[i].c();
    						transition_in(each_blocks[i], 1);
    						each_blocks[i].m(each_1_anchor.parentNode, each_1_anchor);
    					}
    				}

    				group_outros();

    				for (i = each_value.length; i < each_blocks.length; i += 1) {
    					out(i);
    				}

    				check_outros();
    			}
    		},
    		i: function intro(local) {
    			if (current) return;

    			for (let i = 0; i < each_value.length; i += 1) {
    				transition_in(each_blocks[i]);
    			}

    			current = true;
    		},
    		o: function outro(local) {
    			each_blocks = each_blocks.filter(Boolean);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				transition_out(each_blocks[i]);
    			}

    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h1);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(p);
    			if (detaching) detach_dev(t5);
    			if (detaching) detach_dev(div);
    			if (detaching) detach_dev(t8);
    			destroy_each(each_blocks, detaching);
    			if (detaching) detach_dev(each_1_anchor);
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$1.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$1($$self, $$props, $$invalidate) {
    	let data = [];

    	const findData = async param => {
    		try {
    			var descriptions = await fetch(`https://localhost:5001/api/search/${param || "nothing"}`);
    			$$invalidate("data", data = await descriptions.json());
    		} catch(error) {
    			console.log(error);
    		}
    	};

    	const change_handler = e => findData(e.target.value);

    	$$self.$capture_state = () => {
    		return {};
    	};

    	$$self.$inject_state = $$props => {
    		if ("data" in $$props) $$invalidate("data", data = $$props.data);
    	};

    	return { data, findData, change_handler };
    }

    class Main extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$1, create_fragment$1, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "Main",
    			options,
    			id: create_fragment$1.name
    		});
    	}
    }

    const subscriber_queue = [];
    /**
     * Create a `Writable` store that allows both updating and reading by subscription.
     * @param {*=}value initial value
     * @param {StartStopNotifier=}start start and stop notifications for subscriptions
     */
    function writable(value, start = noop) {
        let stop;
        const subscribers = [];
        function set(new_value) {
            if (safe_not_equal(value, new_value)) {
                value = new_value;
                if (stop) { // store is ready
                    const run_queue = !subscriber_queue.length;
                    for (let i = 0; i < subscribers.length; i += 1) {
                        const s = subscribers[i];
                        s[1]();
                        subscriber_queue.push(s, value);
                    }
                    if (run_queue) {
                        for (let i = 0; i < subscriber_queue.length; i += 2) {
                            subscriber_queue[i][0](subscriber_queue[i + 1]);
                        }
                        subscriber_queue.length = 0;
                    }
                }
            }
        }
        function update(fn) {
            set(fn(value));
        }
        function subscribe(run, invalidate = noop) {
            const subscriber = [run, invalidate];
            subscribers.push(subscriber);
            if (subscribers.length === 1) {
                stop = start(set) || noop;
            }
            run(value);
            return () => {
                const index = subscribers.indexOf(subscriber);
                if (index !== -1) {
                    subscribers.splice(index, 1);
                }
                if (subscribers.length === 0) {
                    stop();
                    stop = null;
                }
            };
        }
        return { set, update, subscribe };
    }

    class Navigator {
      constructor() {
        this.route = writable("index");
        this.params = writable([]);
      }
      navigate(route, ...params) {
        this.route.update(n => route);
        this.params.update(n => params || []);
      }

      subscribe(f) {
        this.route.subscribe(f);
      }

      $params(f) {
        this.params.subscribe(f);
      }
    }

    var navigator = new Navigator();

    /* src/Lexicon.svelte generated by Svelte v3.15.0 */
    const file$2 = "src/Lexicon.svelte";

    function get_each_context$1(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.d = list[i];
    	return child_ctx;
    }

    // (98:2) {#each data as d}
    function create_each_block$1(ctx) {
    	let div4;
    	let div0;
    	let t0_value = ctx.d.domain + "";
    	let t0;
    	let t1;
    	let div1;
    	let t2_value = ctx.d.name + "";
    	let t2;
    	let t3;
    	let div3;
    	let t4_value = ctx.d.description + "";
    	let t4;
    	let t5;
    	let div2;
    	let t6;
    	let t7_value = ctx.d.dataOwner + "";
    	let t7;
    	let t8;
    	let span;
    	let t10;
    	let dispose;

    	function click_handler_1(...args) {
    		return ctx.click_handler_1(ctx, ...args);
    	}

    	function click_handler_2(...args) {
    		return ctx.click_handler_2(ctx, ...args);
    	}

    	const block = {
    		c: function create() {
    			div4 = element("div");
    			div0 = element("div");
    			t0 = text(t0_value);
    			t1 = space();
    			div1 = element("div");
    			t2 = text(t2_value);
    			t3 = space();
    			div3 = element("div");
    			t4 = text(t4_value);
    			t5 = space();
    			div2 = element("div");
    			t6 = text("Owner: ");
    			t7 = text(t7_value);
    			t8 = space();
    			span = element("span");
    			span.textContent = "X";
    			t10 = space();
    			attr_dev(div0, "class", "item--name svelte-q471s4");
    			add_location(div0, file$2, 99, 6, 2077);
    			attr_dev(div1, "class", "item--name svelte-q471s4");
    			add_location(div1, file$2, 100, 6, 2124);
    			attr_dev(div2, "class", "item--description__owner svelte-q471s4");
    			add_location(div2, file$2, 107, 8, 2323);
    			attr_dev(div3, "class", "item--description svelte-q471s4");
    			add_location(div3, file$2, 105, 6, 2259);
    			attr_dev(span, "class", "item__delete svelte-q471s4");
    			add_location(span, file$2, 109, 6, 2407);
    			attr_dev(div4, "class", "item svelte-q471s4");
    			add_location(div4, file$2, 98, 4, 2052);

    			dispose = [
    				listen_dev(div1, "click", click_handler_1, false, false, false),
    				listen_dev(span, "click", click_handler_2, false, false, false)
    			];
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div4, anchor);
    			append_dev(div4, div0);
    			append_dev(div0, t0);
    			append_dev(div4, t1);
    			append_dev(div4, div1);
    			append_dev(div1, t2);
    			append_dev(div4, t3);
    			append_dev(div4, div3);
    			append_dev(div3, t4);
    			append_dev(div3, t5);
    			append_dev(div3, div2);
    			append_dev(div2, t6);
    			append_dev(div2, t7);
    			append_dev(div4, t8);
    			append_dev(div4, span);
    			append_dev(div4, t10);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;
    			if (changed.data && t0_value !== (t0_value = ctx.d.domain + "")) set_data_dev(t0, t0_value);
    			if (changed.data && t2_value !== (t2_value = ctx.d.name + "")) set_data_dev(t2, t2_value);
    			if (changed.data && t4_value !== (t4_value = ctx.d.description + "")) set_data_dev(t4, t4_value);
    			if (changed.data && t7_value !== (t7_value = ctx.d.dataOwner + "")) set_data_dev(t7, t7_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div4);
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$1.name,
    		type: "each",
    		source: "(98:2) {#each data as d}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$2(ctx) {
    	let h1;
    	let t1;
    	let div0;
    	let span;
    	let t3;
    	let h2;
    	let t5;
    	let input;
    	let t6;
    	let div5;
    	let div4;
    	let div1;
    	let t8;
    	let div2;
    	let t10;
    	let div3;
    	let t12;
    	let dispose;
    	let each_value = ctx.data;
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$1(get_each_context$1(ctx, each_value, i));
    	}

    	const block = {
    		c: function create() {
    			h1 = element("h1");
    			h1.textContent = "Search your lexicon!";
    			t1 = space();
    			div0 = element("div");
    			span = element("span");
    			span.textContent = "Create";
    			t3 = space();
    			h2 = element("h2");
    			h2.textContent = "Search your lexicon:";
    			t5 = space();
    			input = element("input");
    			t6 = space();
    			div5 = element("div");
    			div4 = element("div");
    			div1 = element("div");
    			div1.textContent = "Domain";
    			t8 = space();
    			div2 = element("div");
    			div2.textContent = "Name";
    			t10 = space();
    			div3 = element("div");
    			div3.textContent = "Description";
    			t12 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			attr_dev(h1, "class", "title");
    			add_location(h1, file$2, 74, 0, 1491);
    			attr_dev(span, "class", "nav-button");
    			add_location(span, file$2, 77, 2, 1544);
    			add_location(h2, file$2, 84, 2, 1667);
    			attr_dev(input, "type", "text");
    			add_location(input, file$2, 85, 2, 1699);
    			add_location(div0, file$2, 76, 0, 1536);
    			attr_dev(div1, "class", "item--name svelte-q471s4");
    			add_location(div1, file$2, 93, 4, 1890);
    			attr_dev(div2, "class", "item--name svelte-q471s4");
    			add_location(div2, file$2, 94, 4, 1931);
    			attr_dev(div3, "class", "item--description svelte-q471s4");
    			add_location(div3, file$2, 95, 4, 1970);
    			attr_dev(div4, "class", "item svelte-q471s4");
    			add_location(div4, file$2, 92, 2, 1867);
    			attr_dev(div5, "class", "items svelte-q471s4");
    			add_location(div5, file$2, 91, 0, 1845);

    			dispose = [
    				listen_dev(span, "click", ctx.click_handler, false, false, false),
    				listen_dev(input, "keyup", ctx.keyup_handler, false, false, false),
    				listen_dev(input, "change", ctx.change_handler, false, false, false)
    			];
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h1, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, div0, anchor);
    			append_dev(div0, span);
    			append_dev(div0, t3);
    			append_dev(div0, h2);
    			append_dev(div0, t5);
    			append_dev(div0, input);
    			insert_dev(target, t6, anchor);
    			insert_dev(target, div5, anchor);
    			append_dev(div5, div4);
    			append_dev(div4, div1);
    			append_dev(div4, t8);
    			append_dev(div4, div2);
    			append_dev(div4, t10);
    			append_dev(div4, div3);
    			append_dev(div5, t12);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div5, null);
    			}
    		},
    		p: function update(changed, ctx) {
    			if (changed.deleteItem || changed.data || changed.navigator) {
    				each_value = ctx.data;
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$1(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(changed, child_ctx);
    					} else {
    						each_blocks[i] = create_each_block$1(child_ctx);
    						each_blocks[i].c();
    						each_blocks[i].m(div5, null);
    					}
    				}

    				for (; i < each_blocks.length; i += 1) {
    					each_blocks[i].d(1);
    				}

    				each_blocks.length = each_value.length;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h1);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(div0);
    			if (detaching) detach_dev(t6);
    			if (detaching) detach_dev(div5);
    			destroy_each(each_blocks, detaching);
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$2.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$2($$self, $$props, $$invalidate) {
    	let data = [];
    	let search = "";

    	let findData = async _search => {
    		if (!_search) return;
    		search = _search;
    		var fetchResult = await fetch("https://localhost:5001/api/lexicon?query=" + search);
    		var result = await fetchResult.json();
    		$$invalidate("data", data = result || []);
    	};

    	let deleteItem = async entry => {
    		var result = await fetch("https://localhost:5001/api/lexicon", {
    			method: "DELETE",
    			headers: { "Content-Type": "application/json" },
    			body: JSON.stringify(entry)
    		});

    		findData(search);
    	};

    	let onkeyup = query => {
    		findData(query);
    	};

    	const click_handler = () => {
    		navigator.navigate("add-lexicon");
    	};

    	const keyup_handler = e => e.code === "Enter" && onkeyup(e.target.value);
    	const change_handler = e => findData(e.target.value);
    	const click_handler_1 = ({ d }) => navigator.navigate("edit-lexicon", d.id);
    	const click_handler_2 = ({ d }) => deleteItem(d);

    	$$self.$capture_state = () => {
    		return {};
    	};

    	$$self.$inject_state = $$props => {
    		if ("data" in $$props) $$invalidate("data", data = $$props.data);
    		if ("search" in $$props) search = $$props.search;
    		if ("findData" in $$props) $$invalidate("findData", findData = $$props.findData);
    		if ("deleteItem" in $$props) $$invalidate("deleteItem", deleteItem = $$props.deleteItem);
    		if ("onkeyup" in $$props) $$invalidate("onkeyup", onkeyup = $$props.onkeyup);
    	};

    	return {
    		data,
    		findData,
    		deleteItem,
    		onkeyup,
    		click_handler,
    		keyup_handler,
    		change_handler,
    		click_handler_1,
    		click_handler_2
    	};
    }

    class Lexicon extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$2, create_fragment$2, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "Lexicon",
    			options,
    			id: create_fragment$2.name
    		});
    	}
    }

    /* src/LexiconAdd.svelte generated by Svelte v3.15.0 */
    const file$3 = "src/LexiconAdd.svelte";

    function get_each_context$2(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.tag = list[i];
    	return child_ctx;
    }

    function get_each_context_1(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.application = list[i];
    	return child_ctx;
    }

    // (156:8) {#each data.applications as application}
    function create_each_block_1(ctx) {
    	let div;
    	let t0_value = ctx.application + "";
    	let t0;
    	let t1;
    	let span;
    	let t3;
    	let dispose;

    	function click_handler(...args) {
    		return ctx.click_handler(ctx, ...args);
    	}

    	const block = {
    		c: function create() {
    			div = element("div");
    			t0 = text(t0_value);
    			t1 = space();
    			span = element("span");
    			span.textContent = "X";
    			t3 = space();
    			attr_dev(span, "class", "delete svelte-1qonizy");
    			add_location(span, file$3, 158, 12, 3225);
    			add_location(div, file$3, 156, 10, 3181);
    			dispose = listen_dev(span, "click", click_handler, false, false, false);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t0);
    			append_dev(div, t1);
    			append_dev(div, span);
    			append_dev(div, t3);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;
    			if (changed.data && t0_value !== (t0_value = ctx.application + "")) set_data_dev(t0, t0_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block_1.name,
    		type: "each",
    		source: "(156:8) {#each data.applications as application}",
    		ctx
    	});

    	return block;
    }

    // (176:8) {#each data.tags as tag}
    function create_each_block$2(ctx) {
    	let div;
    	let t0_value = ctx.tag + "";
    	let t0;
    	let t1;
    	let span;
    	let t3;
    	let dispose;

    	function click_handler_1(...args) {
    		return ctx.click_handler_1(ctx, ...args);
    	}

    	const block = {
    		c: function create() {
    			div = element("div");
    			t0 = text(t0_value);
    			t1 = space();
    			span = element("span");
    			span.textContent = "X";
    			t3 = space();
    			attr_dev(span, "class", "delete svelte-1qonizy");
    			add_location(span, file$3, 178, 12, 3694);
    			add_location(div, file$3, 176, 10, 3658);
    			dispose = listen_dev(span, "click", click_handler_1, false, false, false);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t0);
    			append_dev(div, t1);
    			append_dev(div, span);
    			append_dev(div, t3);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;
    			if (changed.data && t0_value !== (t0_value = ctx.tag + "")) set_data_dev(t0, t0_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$2.name,
    		type: "each",
    		source: "(176:8) {#each data.tags as tag}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$3(ctx) {
    	let h1;
    	let t1;
    	let div10;
    	let form;
    	let div4;
    	let div0;
    	let label0;
    	let t3;
    	let input0;
    	let t4;
    	let div1;
    	let label1;
    	let t6;
    	let input1;
    	let t7;
    	let div2;
    	let label2;
    	let t9;
    	let textarea;
    	let t10;
    	let div3;
    	let label3;
    	let t12;
    	let input2;
    	let t13;
    	let div9;
    	let div6;
    	let label4;
    	let t15;
    	let div5;
    	let input3;
    	let t16;
    	let t17;
    	let div8;
    	let label5;
    	let t19;
    	let div7;
    	let input4;
    	let t20;
    	let t21;
    	let button;
    	let dispose;
    	let each_value_1 = ctx.data.applications;
    	let each_blocks_1 = [];

    	for (let i = 0; i < each_value_1.length; i += 1) {
    		each_blocks_1[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
    	}

    	let each_value = ctx.data.tags;
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$2(get_each_context$2(ctx, each_value, i));
    	}

    	const block = {
    		c: function create() {
    			h1 = element("h1");
    			h1.textContent = "Please describe something";
    			t1 = space();
    			div10 = element("div");
    			form = element("form");
    			div4 = element("div");
    			div0 = element("div");
    			label0 = element("label");
    			label0.textContent = "Domain:";
    			t3 = space();
    			input0 = element("input");
    			t4 = space();
    			div1 = element("div");
    			label1 = element("label");
    			label1.textContent = "Name:";
    			t6 = space();
    			input1 = element("input");
    			t7 = space();
    			div2 = element("div");
    			label2 = element("label");
    			label2.textContent = "Description:";
    			t9 = space();
    			textarea = element("textarea");
    			t10 = space();
    			div3 = element("div");
    			label3 = element("label");
    			label3.textContent = "Data owner:";
    			t12 = space();
    			input2 = element("input");
    			t13 = space();
    			div9 = element("div");
    			div6 = element("div");
    			label4 = element("label");
    			label4.textContent = "Applications:";
    			t15 = space();
    			div5 = element("div");
    			input3 = element("input");
    			t16 = space();

    			for (let i = 0; i < each_blocks_1.length; i += 1) {
    				each_blocks_1[i].c();
    			}

    			t17 = space();
    			div8 = element("div");
    			label5 = element("label");
    			label5.textContent = "Tags:";
    			t19 = space();
    			div7 = element("div");
    			input4 = element("input");
    			t20 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			t21 = space();
    			button = element("button");
    			button.textContent = "Add";
    			attr_dev(h1, "class", "title");
    			add_location(h1, file$3, 112, 0, 2094);
    			attr_dev(label0, "class", "svelte-1qonizy");
    			add_location(label0, file$3, 118, 8, 2202);
    			attr_dev(input0, "class", "svelte-1qonizy");
    			add_location(input0, file$3, 119, 8, 2233);
    			add_location(div0, file$3, 117, 6, 2188);
    			attr_dev(label1, "class", "svelte-1qonizy");
    			add_location(label1, file$3, 125, 8, 2359);
    			attr_dev(input1, "class", "svelte-1qonizy");
    			add_location(input1, file$3, 126, 8, 2388);
    			add_location(div1, file$3, 124, 6, 2345);
    			attr_dev(label2, "class", "svelte-1qonizy");
    			add_location(label2, file$3, 132, 8, 2512);
    			attr_dev(textarea, "class", "svelte-1qonizy");
    			add_location(textarea, file$3, 133, 8, 2548);
    			add_location(div2, file$3, 131, 6, 2498);
    			attr_dev(label3, "class", "svelte-1qonizy");
    			add_location(label3, file$3, 139, 8, 2682);
    			attr_dev(input2, "class", "svelte-1qonizy");
    			add_location(input2, file$3, 140, 8, 2717);
    			add_location(div3, file$3, 138, 6, 2668);
    			attr_dev(div4, "class", "left svelte-1qonizy");
    			add_location(div4, file$3, 116, 4, 2163);
    			attr_dev(label4, "class", "svelte-1qonizy");
    			add_location(label4, file$3, 148, 8, 2909);
    			input3.value = ctx.newApplication;
    			attr_dev(input3, "class", "svelte-1qonizy");
    			add_location(input3, file$3, 150, 10, 2976);
    			attr_dev(div5, "class", "input");
    			add_location(div5, file$3, 149, 8, 2946);
    			set_style(div6, "margin-bottom", "1em");
    			add_location(div6, file$3, 147, 6, 2867);
    			attr_dev(label5, "class", "svelte-1qonizy");
    			add_location(label5, file$3, 168, 8, 3426);
    			input4.value = ctx.newTag;
    			attr_dev(input4, "class", "svelte-1qonizy");
    			add_location(input4, file$3, 170, 10, 3485);
    			attr_dev(div7, "class", "input");
    			add_location(div7, file$3, 169, 8, 3455);
    			add_location(div8, file$3, 167, 6, 3412);
    			attr_dev(div9, "class", "right svelte-1qonizy");
    			add_location(div9, file$3, 146, 4, 2841);
    			attr_dev(form, "class", "svelte-1qonizy");
    			add_location(form, file$3, 115, 2, 2152);
    			attr_dev(button, "type", "button");
    			attr_dev(button, "class", "svelte-1qonizy");
    			add_location(button, file$3, 184, 2, 3825);
    			add_location(div10, file$3, 114, 0, 2144);

    			dispose = [
    				listen_dev(
    					input0,
    					"change",
    					function () {
    						ctx.change_handler.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					input1,
    					"change",
    					function () {
    						ctx.change_handler_1.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					textarea,
    					"change",
    					function () {
    						ctx.change_handler_2.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					input2,
    					"change",
    					function () {
    						ctx.change_handler_3.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(input3, "change", ctx.changeApplication, false, false, false),
    				listen_dev(input3, "keyup", ctx.keyup_handler, false, false, false),
    				listen_dev(input4, "change", ctx.changeTag, false, false, false),
    				listen_dev(input4, "keyup", ctx.keyup_handler_1, false, false, false),
    				listen_dev(button, "click", ctx.submit, false, false, false)
    			];
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h1, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, div10, anchor);
    			append_dev(div10, form);
    			append_dev(form, div4);
    			append_dev(div4, div0);
    			append_dev(div0, label0);
    			append_dev(div0, t3);
    			append_dev(div0, input0);
    			append_dev(div4, t4);
    			append_dev(div4, div1);
    			append_dev(div1, label1);
    			append_dev(div1, t6);
    			append_dev(div1, input1);
    			append_dev(div4, t7);
    			append_dev(div4, div2);
    			append_dev(div2, label2);
    			append_dev(div2, t9);
    			append_dev(div2, textarea);
    			append_dev(div4, t10);
    			append_dev(div4, div3);
    			append_dev(div3, label3);
    			append_dev(div3, t12);
    			append_dev(div3, input2);
    			append_dev(form, t13);
    			append_dev(form, div9);
    			append_dev(div9, div6);
    			append_dev(div6, label4);
    			append_dev(div6, t15);
    			append_dev(div6, div5);
    			append_dev(div5, input3);
    			append_dev(div6, t16);

    			for (let i = 0; i < each_blocks_1.length; i += 1) {
    				each_blocks_1[i].m(div6, null);
    			}

    			append_dev(div9, t17);
    			append_dev(div9, div8);
    			append_dev(div8, label5);
    			append_dev(div8, t19);
    			append_dev(div8, div7);
    			append_dev(div7, input4);
    			append_dev(div8, t20);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div8, null);
    			}

    			append_dev(div10, t21);
    			append_dev(div10, button);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;

    			if (changed.newApplication) {
    				prop_dev(input3, "value", ctx.newApplication);
    			}

    			if (changed.removeApplication || changed.data) {
    				each_value_1 = ctx.data.applications;
    				let i;

    				for (i = 0; i < each_value_1.length; i += 1) {
    					const child_ctx = get_each_context_1(ctx, each_value_1, i);

    					if (each_blocks_1[i]) {
    						each_blocks_1[i].p(changed, child_ctx);
    					} else {
    						each_blocks_1[i] = create_each_block_1(child_ctx);
    						each_blocks_1[i].c();
    						each_blocks_1[i].m(div6, null);
    					}
    				}

    				for (; i < each_blocks_1.length; i += 1) {
    					each_blocks_1[i].d(1);
    				}

    				each_blocks_1.length = each_value_1.length;
    			}

    			if (changed.newTag) {
    				prop_dev(input4, "value", ctx.newTag);
    			}

    			if (changed.removeTag || changed.data) {
    				each_value = ctx.data.tags;
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$2(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(changed, child_ctx);
    					} else {
    						each_blocks[i] = create_each_block$2(child_ctx);
    						each_blocks[i].c();
    						each_blocks[i].m(div8, null);
    					}
    				}

    				for (; i < each_blocks.length; i += 1) {
    					each_blocks[i].d(1);
    				}

    				each_blocks.length = each_value.length;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h1);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(div10);
    			destroy_each(each_blocks_1, detaching);
    			destroy_each(each_blocks, detaching);
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$3.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$3($$self, $$props, $$invalidate) {
    	let data = { tags: [], applications: [] };
    	let newTag = "";
    	let newApplication = "";

    	let submit = async () => {
    		if (!data.name || !data.domain || !data.description) return;

    		await fetch("https://localhost:5001/api/lexicon", {
    			method: "POST",
    			headers: { "Content-Type": "application/json" },
    			body: JSON.stringify(data)
    		});

    		navigator.navigate("lexicon");
    	};

    	let changeTag = e => {
    		$$invalidate("newTag", newTag = e.target.value);
    	};

    	let addTag = () => {
    		$$invalidate("data", data.tags = data.tags || [], data);

    		if (newTag && newTag.length > 0 && data.tags.indexOf(newTag) === -1) {
    			data.tags.push(newTag);
    			$$invalidate("newTag", newTag = "");
    		}
    	};

    	let removeTag = tag => {
    		$$invalidate("data", data.tags = data.tags.filter(t => t !== tag), data);
    	};

    	let changeApplication = e => {
    		$$invalidate("newApplication", newApplication = e.target.value);
    	};

    	let addApplication = () => {
    		$$invalidate("data", data.applications = data.applications || [], data);

    		if (newApplication && newApplication.length > 0 && data.applications.indexOf(newApplication) === -1) {
    			data.applications.push(newApplication);
    			$$invalidate("newApplication", newApplication = "");
    		}
    	};

    	let removeApplication = tag => {
    		$$invalidate("data", data.applications = data.applications.filter(t => t !== tag), data);
    	};

    	let onkeyup = (event, f) => {
    		if (event.code === "Enter") {
    			if (f === "a") addApplication(); else if (f === "t") addTag();
    		}
    	};

    	const change_handler = e => {
    		$$invalidate("data", data.domain = e.target.value, data);
    	};

    	const change_handler_1 = e => {
    		$$invalidate("data", data.name = e.target.value, data);
    	};

    	const change_handler_2 = e => {
    		$$invalidate("data", data.description = e.target.value, data);
    	};

    	const change_handler_3 = e => {
    		$$invalidate("data", data.dataOwner = e.target.value, data);
    	};

    	const keyup_handler = e => onkeyup(e, "a");
    	const click_handler = ({ application }) => removeApplication(application);
    	const keyup_handler_1 = e => onkeyup(e, "t");
    	const click_handler_1 = ({ tag }) => removeTag(tag);

    	$$self.$capture_state = () => {
    		return {};
    	};

    	$$self.$inject_state = $$props => {
    		if ("data" in $$props) $$invalidate("data", data = $$props.data);
    		if ("newTag" in $$props) $$invalidate("newTag", newTag = $$props.newTag);
    		if ("newApplication" in $$props) $$invalidate("newApplication", newApplication = $$props.newApplication);
    		if ("submit" in $$props) $$invalidate("submit", submit = $$props.submit);
    		if ("changeTag" in $$props) $$invalidate("changeTag", changeTag = $$props.changeTag);
    		if ("addTag" in $$props) addTag = $$props.addTag;
    		if ("removeTag" in $$props) $$invalidate("removeTag", removeTag = $$props.removeTag);
    		if ("changeApplication" in $$props) $$invalidate("changeApplication", changeApplication = $$props.changeApplication);
    		if ("addApplication" in $$props) addApplication = $$props.addApplication;
    		if ("removeApplication" in $$props) $$invalidate("removeApplication", removeApplication = $$props.removeApplication);
    		if ("onkeyup" in $$props) $$invalidate("onkeyup", onkeyup = $$props.onkeyup);
    	};

    	return {
    		data,
    		newTag,
    		newApplication,
    		submit,
    		changeTag,
    		removeTag,
    		changeApplication,
    		removeApplication,
    		onkeyup,
    		change_handler,
    		change_handler_1,
    		change_handler_2,
    		change_handler_3,
    		keyup_handler,
    		click_handler,
    		keyup_handler_1,
    		click_handler_1
    	};
    }

    class LexiconAdd extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$3, create_fragment$3, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "LexiconAdd",
    			options,
    			id: create_fragment$3.name
    		});
    	}
    }

    /* src/LexiconEdit.svelte generated by Svelte v3.15.0 */
    const file$4 = "src/LexiconEdit.svelte";

    function get_each_context$3(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.tag = list[i];
    	return child_ctx;
    }

    function get_each_context_1$1(ctx, list, i) {
    	const child_ctx = Object.create(ctx);
    	child_ctx.application = list[i];
    	return child_ctx;
    }

    // (167:8) {#each data.applications as application}
    function create_each_block_1$1(ctx) {
    	let div;
    	let t0_value = ctx.application + "";
    	let t0;
    	let t1;
    	let span;
    	let t3;
    	let dispose;

    	function click_handler(...args) {
    		return ctx.click_handler(ctx, ...args);
    	}

    	const block = {
    		c: function create() {
    			div = element("div");
    			t0 = text(t0_value);
    			t1 = space();
    			span = element("span");
    			span.textContent = "X";
    			t3 = space();
    			attr_dev(span, "class", "delete svelte-1qonizy");
    			add_location(span, file$4, 169, 12, 3549);
    			add_location(div, file$4, 167, 10, 3505);
    			dispose = listen_dev(span, "click", click_handler, false, false, false);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t0);
    			append_dev(div, t1);
    			append_dev(div, span);
    			append_dev(div, t3);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;
    			if (changed.data && t0_value !== (t0_value = ctx.application + "")) set_data_dev(t0, t0_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block_1$1.name,
    		type: "each",
    		source: "(167:8) {#each data.applications as application}",
    		ctx
    	});

    	return block;
    }

    // (187:8) {#each data.tags as tag}
    function create_each_block$3(ctx) {
    	let div;
    	let t0_value = ctx.tag + "";
    	let t0;
    	let t1;
    	let span;
    	let t3;
    	let dispose;

    	function click_handler_1(...args) {
    		return ctx.click_handler_1(ctx, ...args);
    	}

    	const block = {
    		c: function create() {
    			div = element("div");
    			t0 = text(t0_value);
    			t1 = space();
    			span = element("span");
    			span.textContent = "X";
    			t3 = space();
    			attr_dev(span, "class", "delete svelte-1qonizy");
    			add_location(span, file$4, 189, 12, 4018);
    			add_location(div, file$4, 187, 10, 3982);
    			dispose = listen_dev(span, "click", click_handler_1, false, false, false);
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, div, anchor);
    			append_dev(div, t0);
    			append_dev(div, t1);
    			append_dev(div, span);
    			append_dev(div, t3);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;
    			if (changed.data && t0_value !== (t0_value = ctx.tag + "")) set_data_dev(t0, t0_value);
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(div);
    			dispose();
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_each_block$3.name,
    		type: "each",
    		source: "(187:8) {#each data.tags as tag}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$4(ctx) {
    	let h1;
    	let t1;
    	let div10;
    	let form;
    	let div4;
    	let div0;
    	let label0;
    	let t3;
    	let input0;
    	let input0_value_value;
    	let t4;
    	let div1;
    	let label1;
    	let t6;
    	let input1;
    	let input1_value_value;
    	let t7;
    	let div2;
    	let label2;
    	let t9;
    	let textarea;
    	let textarea_value_value;
    	let t10;
    	let div3;
    	let label3;
    	let t12;
    	let input2;
    	let input2_value_value;
    	let t13;
    	let div9;
    	let div6;
    	let label4;
    	let t15;
    	let div5;
    	let input3;
    	let t16;
    	let t17;
    	let div8;
    	let label5;
    	let t19;
    	let div7;
    	let input4;
    	let t20;
    	let t21;
    	let button;
    	let dispose;
    	let each_value_1 = ctx.data.applications;
    	let each_blocks_1 = [];

    	for (let i = 0; i < each_value_1.length; i += 1) {
    		each_blocks_1[i] = create_each_block_1$1(get_each_context_1$1(ctx, each_value_1, i));
    	}

    	let each_value = ctx.data.tags;
    	let each_blocks = [];

    	for (let i = 0; i < each_value.length; i += 1) {
    		each_blocks[i] = create_each_block$3(get_each_context$3(ctx, each_value, i));
    	}

    	const block = {
    		c: function create() {
    			h1 = element("h1");
    			h1.textContent = "Edit your something";
    			t1 = space();
    			div10 = element("div");
    			form = element("form");
    			div4 = element("div");
    			div0 = element("div");
    			label0 = element("label");
    			label0.textContent = "Domain:";
    			t3 = space();
    			input0 = element("input");
    			t4 = space();
    			div1 = element("div");
    			label1 = element("label");
    			label1.textContent = "Name:";
    			t6 = space();
    			input1 = element("input");
    			t7 = space();
    			div2 = element("div");
    			label2 = element("label");
    			label2.textContent = "Description:";
    			t9 = space();
    			textarea = element("textarea");
    			t10 = space();
    			div3 = element("div");
    			label3 = element("label");
    			label3.textContent = "Data owner:";
    			t12 = space();
    			input2 = element("input");
    			t13 = space();
    			div9 = element("div");
    			div6 = element("div");
    			label4 = element("label");
    			label4.textContent = "Applications:";
    			t15 = space();
    			div5 = element("div");
    			input3 = element("input");
    			t16 = space();

    			for (let i = 0; i < each_blocks_1.length; i += 1) {
    				each_blocks_1[i].c();
    			}

    			t17 = space();
    			div8 = element("div");
    			label5 = element("label");
    			label5.textContent = "Tags:";
    			t19 = space();
    			div7 = element("div");
    			input4 = element("input");
    			t20 = space();

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].c();
    			}

    			t21 = space();
    			button = element("button");
    			button.textContent = "Save";
    			attr_dev(h1, "class", "title");
    			add_location(h1, file$4, 119, 0, 2298);
    			attr_dev(label0, "class", "svelte-1qonizy");
    			add_location(label0, file$4, 125, 8, 2400);
    			input0.value = input0_value_value = ctx.data.domain;
    			attr_dev(input0, "class", "svelte-1qonizy");
    			add_location(input0, file$4, 126, 8, 2431);
    			add_location(div0, file$4, 124, 6, 2386);
    			attr_dev(label1, "class", "svelte-1qonizy");
    			add_location(label1, file$4, 133, 8, 2587);
    			input1.value = input1_value_value = ctx.data.name;
    			attr_dev(input1, "class", "svelte-1qonizy");
    			add_location(input1, file$4, 134, 8, 2616);
    			add_location(div1, file$4, 132, 6, 2573);
    			attr_dev(label2, "class", "svelte-1qonizy");
    			add_location(label2, file$4, 141, 8, 2768);
    			textarea.value = textarea_value_value = ctx.data.description;
    			attr_dev(textarea, "class", "svelte-1qonizy");
    			add_location(textarea, file$4, 142, 8, 2804);
    			add_location(div2, file$4, 140, 6, 2754);
    			attr_dev(label3, "class", "svelte-1qonizy");
    			add_location(label3, file$4, 149, 8, 2973);
    			input2.value = input2_value_value = ctx.data.dataOwner;
    			attr_dev(input2, "class", "svelte-1qonizy");
    			add_location(input2, file$4, 150, 8, 3008);
    			add_location(div3, file$4, 148, 6, 2959);
    			attr_dev(div4, "class", "left svelte-1qonizy");
    			add_location(div4, file$4, 123, 4, 2361);
    			attr_dev(label4, "class", "svelte-1qonizy");
    			add_location(label4, file$4, 159, 8, 3233);
    			input3.value = ctx.newApplication;
    			attr_dev(input3, "class", "svelte-1qonizy");
    			add_location(input3, file$4, 161, 10, 3300);
    			attr_dev(div5, "class", "input");
    			add_location(div5, file$4, 160, 8, 3270);
    			set_style(div6, "margin-bottom", "1em");
    			add_location(div6, file$4, 158, 6, 3191);
    			attr_dev(label5, "class", "svelte-1qonizy");
    			add_location(label5, file$4, 179, 8, 3750);
    			input4.value = ctx.newTag;
    			attr_dev(input4, "class", "svelte-1qonizy");
    			add_location(input4, file$4, 181, 10, 3809);
    			attr_dev(div7, "class", "input");
    			add_location(div7, file$4, 180, 8, 3779);
    			add_location(div8, file$4, 178, 6, 3736);
    			attr_dev(div9, "class", "right svelte-1qonizy");
    			add_location(div9, file$4, 157, 4, 3165);
    			attr_dev(form, "class", "svelte-1qonizy");
    			add_location(form, file$4, 122, 2, 2350);
    			attr_dev(button, "type", "button");
    			attr_dev(button, "class", "svelte-1qonizy");
    			add_location(button, file$4, 195, 2, 4149);
    			add_location(div10, file$4, 121, 0, 2342);

    			dispose = [
    				listen_dev(
    					input0,
    					"change",
    					function () {
    						ctx.change_handler.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					input1,
    					"change",
    					function () {
    						ctx.change_handler_1.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					textarea,
    					"change",
    					function () {
    						ctx.change_handler_2.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(
    					input2,
    					"change",
    					function () {
    						ctx.change_handler_3.apply(this, arguments);
    					},
    					false,
    					false,
    					false
    				),
    				listen_dev(input3, "change", ctx.changeApplication, false, false, false),
    				listen_dev(input3, "keyup", ctx.keyup_handler, false, false, false),
    				listen_dev(input4, "change", ctx.changeTag, false, false, false),
    				listen_dev(input4, "keyup", ctx.keyup_handler_1, false, false, false),
    				listen_dev(button, "click", ctx.submit, false, false, false)
    			];
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, h1, anchor);
    			insert_dev(target, t1, anchor);
    			insert_dev(target, div10, anchor);
    			append_dev(div10, form);
    			append_dev(form, div4);
    			append_dev(div4, div0);
    			append_dev(div0, label0);
    			append_dev(div0, t3);
    			append_dev(div0, input0);
    			append_dev(div4, t4);
    			append_dev(div4, div1);
    			append_dev(div1, label1);
    			append_dev(div1, t6);
    			append_dev(div1, input1);
    			append_dev(div4, t7);
    			append_dev(div4, div2);
    			append_dev(div2, label2);
    			append_dev(div2, t9);
    			append_dev(div2, textarea);
    			append_dev(div4, t10);
    			append_dev(div4, div3);
    			append_dev(div3, label3);
    			append_dev(div3, t12);
    			append_dev(div3, input2);
    			append_dev(form, t13);
    			append_dev(form, div9);
    			append_dev(div9, div6);
    			append_dev(div6, label4);
    			append_dev(div6, t15);
    			append_dev(div6, div5);
    			append_dev(div5, input3);
    			append_dev(div6, t16);

    			for (let i = 0; i < each_blocks_1.length; i += 1) {
    				each_blocks_1[i].m(div6, null);
    			}

    			append_dev(div9, t17);
    			append_dev(div9, div8);
    			append_dev(div8, label5);
    			append_dev(div8, t19);
    			append_dev(div8, div7);
    			append_dev(div7, input4);
    			append_dev(div8, t20);

    			for (let i = 0; i < each_blocks.length; i += 1) {
    				each_blocks[i].m(div8, null);
    			}

    			append_dev(div10, t21);
    			append_dev(div10, button);
    		},
    		p: function update(changed, new_ctx) {
    			ctx = new_ctx;

    			if (changed.data && input0_value_value !== (input0_value_value = ctx.data.domain)) {
    				prop_dev(input0, "value", input0_value_value);
    			}

    			if (changed.data && input1_value_value !== (input1_value_value = ctx.data.name)) {
    				prop_dev(input1, "value", input1_value_value);
    			}

    			if (changed.data && textarea_value_value !== (textarea_value_value = ctx.data.description)) {
    				prop_dev(textarea, "value", textarea_value_value);
    			}

    			if (changed.data && input2_value_value !== (input2_value_value = ctx.data.dataOwner)) {
    				prop_dev(input2, "value", input2_value_value);
    			}

    			if (changed.newApplication) {
    				prop_dev(input3, "value", ctx.newApplication);
    			}

    			if (changed.removeApplication || changed.data) {
    				each_value_1 = ctx.data.applications;
    				let i;

    				for (i = 0; i < each_value_1.length; i += 1) {
    					const child_ctx = get_each_context_1$1(ctx, each_value_1, i);

    					if (each_blocks_1[i]) {
    						each_blocks_1[i].p(changed, child_ctx);
    					} else {
    						each_blocks_1[i] = create_each_block_1$1(child_ctx);
    						each_blocks_1[i].c();
    						each_blocks_1[i].m(div6, null);
    					}
    				}

    				for (; i < each_blocks_1.length; i += 1) {
    					each_blocks_1[i].d(1);
    				}

    				each_blocks_1.length = each_value_1.length;
    			}

    			if (changed.newTag) {
    				prop_dev(input4, "value", ctx.newTag);
    			}

    			if (changed.removeTag || changed.data) {
    				each_value = ctx.data.tags;
    				let i;

    				for (i = 0; i < each_value.length; i += 1) {
    					const child_ctx = get_each_context$3(ctx, each_value, i);

    					if (each_blocks[i]) {
    						each_blocks[i].p(changed, child_ctx);
    					} else {
    						each_blocks[i] = create_each_block$3(child_ctx);
    						each_blocks[i].c();
    						each_blocks[i].m(div8, null);
    					}
    				}

    				for (; i < each_blocks.length; i += 1) {
    					each_blocks[i].d(1);
    				}

    				each_blocks.length = each_value.length;
    			}
    		},
    		i: noop,
    		o: noop,
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(h1);
    			if (detaching) detach_dev(t1);
    			if (detaching) detach_dev(div10);
    			destroy_each(each_blocks_1, detaching);
    			destroy_each(each_blocks, detaching);
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$4.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$4($$self, $$props, $$invalidate) {
    	let data = { tags: [], applications: [] };

    	let fetchData = async params => {
    		var query = await fetch("https://localhost:5001/api/lexicon/" + params);
    		var _data = await query.json();
    		$$invalidate("data", data = _data);
    	};

    	navigator.$params(fetchData);
    	let newTag = "";
    	let newApplication = "";

    	let submit = async () => {
    		if (!data.name || !data.domain || !data.description) return;

    		await fetch("https://localhost:5001/api/lexicon", {
    			method: "PUT",
    			headers: { "Content-Type": "application/json" },
    			body: JSON.stringify(data)
    		});

    		navigator.navigate("lexicon");
    	};

    	let changeTag = e => {
    		$$invalidate("newTag", newTag = e.target.value);
    	};

    	let addTag = () => {
    		$$invalidate("data", data.tags = data.tags || [], data);

    		if (newTag && newTag.length > 0 && data.tags.indexOf(newTag) === -1) {
    			data.tags.push(newTag);
    			$$invalidate("newTag", newTag = "");
    		}
    	};

    	let removeTag = tag => {
    		$$invalidate("data", data.tags = data.tags.filter(t => t !== tag), data);
    	};

    	let changeApplication = e => {
    		$$invalidate("newApplication", newApplication = e.target.value);
    	};

    	let addApplication = () => {
    		$$invalidate("data", data.applications = data.applications || [], data);

    		if (newApplication && newApplication.length > 0 && data.applications.indexOf(newApplication) === -1) {
    			data.applications.push(newApplication);
    			$$invalidate("newApplication", newApplication = "");
    		}
    	};

    	let removeApplication = tag => {
    		$$invalidate("data", data.applications = data.applications.filter(t => t !== tag), data);
    	};

    	let onkeyup = (event, f) => {
    		if (event.code === "Enter") {
    			if (f === "a") addApplication(); else if (f === "t") addTag();
    		}
    	};

    	const change_handler = e => {
    		$$invalidate("data", data.domain = e.target.value, data);
    	};

    	const change_handler_1 = e => {
    		$$invalidate("data", data.name = e.target.value, data);
    	};

    	const change_handler_2 = e => {
    		$$invalidate("data", data.description = e.target.value, data);
    	};

    	const change_handler_3 = e => {
    		$$invalidate("data", data.dataOwner = e.target.value, data);
    	};

    	const keyup_handler = e => onkeyup(e, "a");
    	const click_handler = ({ application }) => removeApplication(application);
    	const keyup_handler_1 = e => onkeyup(e, "t");
    	const click_handler_1 = ({ tag }) => removeTag(tag);

    	$$self.$capture_state = () => {
    		return {};
    	};

    	$$self.$inject_state = $$props => {
    		if ("data" in $$props) $$invalidate("data", data = $$props.data);
    		if ("fetchData" in $$props) fetchData = $$props.fetchData;
    		if ("newTag" in $$props) $$invalidate("newTag", newTag = $$props.newTag);
    		if ("newApplication" in $$props) $$invalidate("newApplication", newApplication = $$props.newApplication);
    		if ("submit" in $$props) $$invalidate("submit", submit = $$props.submit);
    		if ("changeTag" in $$props) $$invalidate("changeTag", changeTag = $$props.changeTag);
    		if ("addTag" in $$props) addTag = $$props.addTag;
    		if ("removeTag" in $$props) $$invalidate("removeTag", removeTag = $$props.removeTag);
    		if ("changeApplication" in $$props) $$invalidate("changeApplication", changeApplication = $$props.changeApplication);
    		if ("addApplication" in $$props) addApplication = $$props.addApplication;
    		if ("removeApplication" in $$props) $$invalidate("removeApplication", removeApplication = $$props.removeApplication);
    		if ("onkeyup" in $$props) $$invalidate("onkeyup", onkeyup = $$props.onkeyup);
    	};

    	return {
    		data,
    		newTag,
    		newApplication,
    		submit,
    		changeTag,
    		removeTag,
    		changeApplication,
    		removeApplication,
    		onkeyup,
    		change_handler,
    		change_handler_1,
    		change_handler_2,
    		change_handler_3,
    		keyup_handler,
    		click_handler,
    		keyup_handler_1,
    		click_handler_1
    	};
    }

    class LexiconEdit extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$4, create_fragment$4, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "LexiconEdit",
    			options,
    			id: create_fragment$4.name
    		});
    	}
    }

    /* src/App.svelte generated by Svelte v3.15.0 */
    const file$5 = "src/App.svelte";

    // (46:2) {:else}
    function create_else_block(ctx) {
    	let current;
    	const main = new Main({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(main.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(main, target, anchor);
    			current = true;
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(main.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(main.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(main, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_else_block.name,
    		type: "else",
    		source: "(46:2) {:else}",
    		ctx
    	});

    	return block;
    }

    // (44:37) 
    function create_if_block_3(ctx) {
    	let current;
    	const lexiconedit = new LexiconEdit({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(lexiconedit.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(lexiconedit, target, anchor);
    			current = true;
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(lexiconedit.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(lexiconedit.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(lexiconedit, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block_3.name,
    		type: "if",
    		source: "(44:37) ",
    		ctx
    	});

    	return block;
    }

    // (42:36) 
    function create_if_block_2(ctx) {
    	let current;
    	const lexiconadd = new LexiconAdd({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(lexiconadd.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(lexiconadd, target, anchor);
    			current = true;
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(lexiconadd.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(lexiconadd.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(lexiconadd, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block_2.name,
    		type: "if",
    		source: "(42:36) ",
    		ctx
    	});

    	return block;
    }

    // (40:32) 
    function create_if_block_1(ctx) {
    	let current;
    	const lexicon = new Lexicon({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(lexicon.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(lexicon, target, anchor);
    			current = true;
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(lexicon.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(lexicon.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(lexicon, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block_1.name,
    		type: "if",
    		source: "(40:32) ",
    		ctx
    	});

    	return block;
    }

    // (38:2) {#if route === 'index'}
    function create_if_block$1(ctx) {
    	let current;
    	const main = new Main({ $$inline: true });

    	const block = {
    		c: function create() {
    			create_component(main.$$.fragment);
    		},
    		m: function mount(target, anchor) {
    			mount_component(main, target, anchor);
    			current = true;
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(main.$$.fragment, local);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(main.$$.fragment, local);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			destroy_component(main, detaching);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_if_block$1.name,
    		type: "if",
    		source: "(38:2) {#if route === 'index'}",
    		ctx
    	});

    	return block;
    }

    function create_fragment$5(ctx) {
    	let main;
    	let span0;
    	let t1;
    	let span1;
    	let t3;
    	let current_block_type_index;
    	let if_block;
    	let current;
    	let dispose;

    	const if_block_creators = [
    		create_if_block$1,
    		create_if_block_1,
    		create_if_block_2,
    		create_if_block_3,
    		create_else_block
    	];

    	const if_blocks = [];

    	function select_block_type(changed, ctx) {
    		if (ctx.route === "index") return 0;
    		if (ctx.route === "lexicon") return 1;
    		if (ctx.route === "add-lexicon") return 2;
    		if (ctx.route === "edit-lexicon") return 3;
    		return 4;
    	}

    	current_block_type_index = select_block_type(null, ctx);
    	if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);

    	const block = {
    		c: function create() {
    			main = element("main");
    			span0 = element("span");
    			span0.textContent = "Home";
    			t1 = space();
    			span1 = element("span");
    			span1.textContent = "Lexicon";
    			t3 = space();
    			if_block.c();
    			attr_dev(span0, "class", "nav-button");
    			add_location(span0, file$5, 30, 2, 530);
    			attr_dev(span1, "class", "nav-button");
    			add_location(span1, file$5, 33, 2, 622);
    			attr_dev(main, "class", "svelte-1kds1eb");
    			add_location(main, file$5, 28, 0, 520);

    			dispose = [
    				listen_dev(span0, "click", ctx.click_handler, false, false, false),
    				listen_dev(span1, "click", ctx.click_handler_1, false, false, false)
    			];
    		},
    		l: function claim(nodes) {
    			throw new Error("options.hydrate only works if the component was compiled with the `hydratable: true` option");
    		},
    		m: function mount(target, anchor) {
    			insert_dev(target, main, anchor);
    			append_dev(main, span0);
    			append_dev(main, t1);
    			append_dev(main, span1);
    			append_dev(main, t3);
    			if_blocks[current_block_type_index].m(main, null);
    			current = true;
    		},
    		p: function update(changed, ctx) {
    			let previous_block_index = current_block_type_index;
    			current_block_type_index = select_block_type(changed, ctx);

    			if (current_block_type_index !== previous_block_index) {
    				group_outros();

    				transition_out(if_blocks[previous_block_index], 1, 1, () => {
    					if_blocks[previous_block_index] = null;
    				});

    				check_outros();
    				if_block = if_blocks[current_block_type_index];

    				if (!if_block) {
    					if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
    					if_block.c();
    				}

    				transition_in(if_block, 1);
    				if_block.m(main, null);
    			}
    		},
    		i: function intro(local) {
    			if (current) return;
    			transition_in(if_block);
    			current = true;
    		},
    		o: function outro(local) {
    			transition_out(if_block);
    			current = false;
    		},
    		d: function destroy(detaching) {
    			if (detaching) detach_dev(main);
    			if_blocks[current_block_type_index].d();
    			run_all(dispose);
    		}
    	};

    	dispatch_dev("SvelteRegisterBlock", {
    		block,
    		id: create_fragment$5.name,
    		type: "component",
    		source: "",
    		ctx
    	});

    	return block;
    }

    function instance$5($$self, $$props, $$invalidate) {
    	let route;

    	const unsubscribe = navigator.subscribe(value => {
    		$$invalidate("route", route = value);
    	});

    	const click_handler = () => navigator.navigate("index");
    	const click_handler_1 = () => navigator.navigate("lexicon");

    	$$self.$capture_state = () => {
    		return {};
    	};

    	$$self.$inject_state = $$props => {
    		if ("route" in $$props) $$invalidate("route", route = $$props.route);
    	};

    	return { route, click_handler, click_handler_1 };
    }

    class App extends SvelteComponentDev {
    	constructor(options) {
    		super(options);
    		init(this, options, instance$5, create_fragment$5, safe_not_equal, {});

    		dispatch_dev("SvelteRegisterComponent", {
    			component: this,
    			tagName: "App",
    			options,
    			id: create_fragment$5.name
    		});
    	}
    }

    const app = new App({
    	target: document.body,
    	props: {
    		name: 'world'
    	}
    });

    return app;

}());
//# sourceMappingURL=bundle.js.map
