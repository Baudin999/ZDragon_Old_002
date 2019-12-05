<script>
  import { onMount } from "svelte";
  export let show = false;

  let keyup = e => {
    if (e.code == "Enter" || e.code == "Escape") {
      show = false;
    }
  };

  onMount(() => {
    focus();
  });

  let focus = () => {
    document.getElementById("command-input").focus();
  };
</script>

<style>
  .overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    height: 100%;
    width: 100%;
    background: rgba(0, 0, 0, 0.4);
    visibility: hidden;
  }
  .overlay.show {
    visibility: visible;
  }
  .overlay .window {
    background: #313338;
    border: 1px solid 616265;
    border-radius: 5px;
    position: absolute;
    top: 10%;
    width: 300px;
    left: 50%;
    transform: translateX(-50%);
    font-size: 2em;
    color: white;
  }
</style>

<div class="overlay {show ? 'show' : 'hide'}">
  <div class="window">
    <input id="command-input" on:keyup={e => keyup(e)} on:blur={focus} />
  </div>
</div>
