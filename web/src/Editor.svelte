<script>
  import { onMount } from "svelte";
  import navigator from "./navigator.js";

  let flask;

  let getcode = async () => {
    var codeRequest = await fetch(
      "https://localhost:5001/api/module/" + navigator.module
    );
    var code = await codeRequest.text();
    flask.updateCode(code);
  };

  let saveCode = async () => {
    var code = flask.getCode();
    var codeRequest = await fetch(
      "https://localhost:5001/api/module/" + navigator.module,
      {
        method: "POST",
        headers: {
          "Content-Type": "text/plain",
          "Content-Length": code.length.toString()
        },
        body: code
      }
    );
  };

  onMount(() => {
    flask = new CodeFlask("#editor", {
      language: "carlang",
      tabSize: 4
    });
    getcode();

    window.addEventListener("keydown", function(e) {
      let key = String.fromCharCode(event.which).toLowerCase();
      if (key === "s" && e.metaKey === true) {
        saveCode();
        e.preventDefault();
        return false;
      }
    });
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

<div class="editor-container">
  <div id="editor" />
</div>
