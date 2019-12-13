<script>
  import { onMount, onDestroy } from "svelte";
  import navigator from "./navigator.js";
  import { getCode } from "./Services/codeServices";

  let errors = [];
  let flask;
  let showPreview = false;
  let route = `/${navigator.module}/index.html`;

  let getcode = async () => {
    // var codeRequest = await fetch("/api/module/" + navigator.module);
    // var code = await codeRequest.text();
    debugger;
    var code = await getCode();
    flask.updateCode(code);
    setTimeout(getErrors, 0);
  };

  let saveCode = async () => {
    var code = flask.getCode();
    var codeRequest = await fetch("/api/module/" + navigator.module, {
      method: "POST",
      headers: {
        "Content-Type": "text/plain",
        "Content-Length": code.length.toString()
      },
      body: code
    });
    setTimeout(getErrors, 2000);
    setTimeout(() => {
      var iframe = document.getElementById("iframe--preview");
      iframe.src = iframe.src;
    }, 2000);
  };

  let getErrors = async () => {
    var errorsRequest = await fetch(
      "/api/module/" + navigator.module + "/errors"
    );
    errors = await errorsRequest.json();
    console.log(errors);
  };

  function mouseTrap(e) {
    if (e.key === "s" && (e.ctrlKey === true || e.metaKey == true)) {
      saveCode();
      e.preventDefault();
      return false;
    } else if (e.key === "e" && (e.ctrlKey === true || e.metaKey == true)) {
      console.log("showing preview");
      showPreview = !showPreview;
      e.preventDefault();
      return false;
    }
  }

  onMount(() => {
    flask = new CodeFlask("#editor", {
      language: "carlang",
      tabSize: 4
    });
    getcode();

    window.addEventListener("keydown", mouseTrap, true);
  });
  onDestroy(() => {
    window.removeEventListener("keydown", mouseTrap, true);
  });
</script>

<style>
  .errors {
    position: fixed;
    right: 1rem;
    top: 6rem;
  }
  .error {
    min-width: 300px;
    max-width: 700px;
  }
  .error pre {
    overflow-wrap: break-word;
  }

  .editor textarea {
    max-width: none !important;
  }

  .preview {
    position: fixed;
    right: 1rem;
    top: 4rem;
    bottom: 1rem;
    width: calc(100% - 400px);
    min-width: 700px;
    -webkit-box-shadow: 0px 0px 15px 4px rgba(0, 0, 0, 0.75);
    -moz-box-shadow: 0px 0px 15px 4px rgba(0, 0, 0, 0.75);
    box-shadow: 0px 0px 15px 4px rgba(0, 0, 0, 0.75);
    z-index: 999;
    background: white;
    padding: 1em;
  }
  .preview iframe {
    height: 100%;
    border: none;
    width: 100%;
  }
</style>

<div class="content--center">
  <span class="nav-button" on:click={saveCode}>Save</span>
</div>
<div class="editor-container">
  <div id="editor" />
</div>

{#if errors && errors.length > 0}
  <div class="errors">
    {#each errors as error}
      <div class="error">
        <div class="title">{error.title}</div>
        <pre>{error.message}</pre>
      </div>
    {/each}
  </div>
{/if}

{#if showPreview}
  <div class="preview">
    <iframe id="iframe--preview" title="preview" src={route} />
  </div>
{/if}
