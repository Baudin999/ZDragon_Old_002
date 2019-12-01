<script>
  import Main from "./Main.svelte";
  import Lexicon from "./Lexicon.svelte";
  import LexiconAdd from "./LexiconAdd.svelte";
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

  .nav-button {
    display: inline-block;
    margin: 0 1em;
    color: rgb(15, 82, 109);
  }
  .nav-button:hover {
    cursor: pointer;
  }
</style>

<main>

  <span class="nav-button" on:click={() => navigator.navigate('index')}>
    Home
  </span>
  <span class="nav-button" on:click={() => navigator.navigate('lexicon')}>
    Lexicon
  </span>

  {#if route === 'index'}
    <Main />
  {:else if route === 'lexicon'}
    <Lexicon />
  {:else if route === 'add-lexicon'}
    <LexiconAdd />
  {:else}
    <Main />
  {/if}

</main>
