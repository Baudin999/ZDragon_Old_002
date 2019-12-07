<script>
  import { onMount, onDestroy } from "svelte";
  import navigator from "./navigator.js";

  let errors = [];
  let flask;

  let getcode = async () => {
    var codeRequest = await fetch("/api/module/" + navigator.module);
    var code = await codeRequest.text();
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
    setTimeout(getErrors, 1000);
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
    }
  }

  onMount(() => {
    flask = new CodeFlask("#editor", {
      language: "carlang",
      tabSize: 4,
      lineNumbers: true
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
    width: 300px;
  }
  .error pre {
    overflow-wrap: break-word;
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
