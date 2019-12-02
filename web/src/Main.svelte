<script>
  import SearchResult from "./SearchResult.svelte";
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

</style>

<h1 class="title">Welcome to ZDragon!</h1>
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
