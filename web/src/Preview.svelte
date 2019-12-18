<script>
  import navigator from "./navigator.js";
  let route = `/${navigator.module}/index.html`;
</script>

<style>
  .preview {
    height: calc(100% - 60px);
  }
  iframe {
    height: 100%;
    width: 23cm;
    padding: 0 1cm;
    border: none;
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
  }
</style>

<div class="preview">
  <iframe title="module_preview" src={route} />
</div>
