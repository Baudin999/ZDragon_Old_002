<script>
  import { onMount, onDestroy } from "svelte";
  import navigator from "./navigator.js";

  let flask;

  let getcode = async () => {
    var codeRequest = await fetch("/api/module/" + navigator.module);
    var code = await codeRequest.text();
    flask.updateCode(code);
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
  };

  function keypress(e) {
    let key = String.fromCharCode(event.which).toLowerCase();
    if (key === "s" && e.metaKey === true) {
      saveCode();
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
    window.addEventListener("keypress", keypress);
  });
  onDestroy(() => {
    window.removeEventListener("keypress", keypress);
  });
</script>

<style>
  .editor-container {
    width: 100%;
  }
  .editor-container .editor {
    width: 100%;
  }
</style>

<span class="nav-button" on:click={saveCode}>Save</span>

<div class="editor-container">
  <div id="editor" />
</div>
