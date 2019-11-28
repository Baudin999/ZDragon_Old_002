<script>
  import SearchResult from "./SearchResult.svelte";
  export let name;

  let data = [];

  const findData = async param => {
    try {
      var descriptions = await fetch(
        `https://localhost:5001/api/search/${param || "nothing"}`
      );
      data = await descriptions.json();
    } catch (error) {
      console.log(error);
    }
  };
</script>

<style>
  main {
    text-align: center;
    padding: 1em;
    max-width: 240px;
    margin: 0 auto;
  }

  h1 {
    color: #ff3e00;
    text-transform: uppercase;
    font-size: 4em;
    font-weight: 100;
  }

  @media (min-width: 640px) {
    main {
      max-width: none;
    }
  }
</style>

<main>
  <h1>Welcome to ZDragon!</h1>
  <p>
    Visit
    <a href="https://zdragon.nl">ZDragon.nl</a>
    to learn more about zdragon!
  </p>

  <div>
    <h2>Search your models:</h2>
    <input type="text" on:change={e => findData(e.target.value)} />
  </div>

  {#each data as d}
    <SearchResult descriptor={d} />
  {/each}

</main>
