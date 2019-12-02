import { writable } from "svelte/store";

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

export default new Navigator();
