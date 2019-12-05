<script>
  import Overlay from "./Controls/Overlay.svelte";
  export let showOverlay = false;
  let tree = {
    root: {
      files: [
        {
          name: "School",
          files: [{ name: "Service" }]
        }
      ]
    }
  };
</script>

<style>
  ul {
    list-style: none;
    text-align: left;
  }
</style>

<ul>
  {#each tree.root.files as file}
    <li
      on:click={() => {
        showOverlay = !showOverlay;
      }}>
      {file.name}
      <ul>
        {#each file.files as nested}
          <li>{nested.name}</li>
        {/each}
      </ul>
    </li>
  {/each}
</ul>
<Overlay show={showOverlay} />
