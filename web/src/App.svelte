<script>
  import Home from "./Home.svelte";
  import Lexicon from "./Lexicon.svelte";
  import LexiconAdd from "./LexiconAdd.svelte";
  import LexiconEdit from "./LexiconEdit.svelte";
  import Editor from "./Editor.svelte";
  import LexiconAdmin from "./LexiconAdmin.svelte";
  import Preview from "./Preview.svelte";
  import ZDragonConfig from "./ZDragonConfig.svelte";
  import ModuleTopology from "./ModuleTopology.svelte";
  import navigator from "./navigator.js";

  let route;

  const unsubscribe = navigator.subscribe(value => {
    route = value;
  });
</script>

<style>
  main {
    text-align: center;
    padding: 1em;
    max-width: 240px;
    margin: 0 auto;
  }
  @media (min-width: 640px) {
    main {
      max-width: none;
    }
  }
</style>

<main>

  <span class="nav-button" on:click={() => navigator.navigate('index')}>
    Home
  </span>
  <span
    class="nav-button"
    on:click={() => navigator.navigate('module-topology')}>
    Topology
  </span>
  <span class="nav-button" on:click={() => navigator.navigate('lexicon')}>
    Lexicon
  </span>
  {#if navigator.module}
    <span class="nav-button" on:click={() => navigator.navigate('editor')}>
      Editor
    </span>
    <span class="nav-button" on:click={() => navigator.navigate('preview')}>
      Preview
    </span>
  {/if}
</main>

{#if route === 'index'}
  <Home />
{:else if route === 'lexicon'}
  <Lexicon />
{:else if route === 'add-lexicon'}
  <LexiconAdd />
{:else if route === 'edit-lexicon'}
  <LexiconEdit />
{:else if route === 'lexicon-admin'}
  <LexiconAdmin />
{:else if route === 'preview'}
  <Preview />
{:else if route === 'editor'}
  <Editor />
{:else if route === 'config'}
  <ZDragonConfig />
{:else if route === 'module-topology'}
  <ModuleTopology />
{:else}
  <Home />
{/if}
