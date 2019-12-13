<script>
  import navigator from "./navigator.js";
  let route = `/${navigator.module}/index.html`;
</script>

<style>
  .preview {
    height: calc(100% - 60px);
  }
  iframe {
    width: 100%;
    height: 100%;
    border: none;
  }
</style>

<div class="preview">
  <iframe title="module_preview" src={route} />
</div>
