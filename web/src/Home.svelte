<script>
  import SearchResult from "./SearchResult.svelte";
  import FileTree from "./FileTree.svelte";
  import HomeHelp from "./Controls/HomeHelp.svelte";
  import navigator from "./navigator.js";

  let storedData = sessionStorage.getItem("search-results");

  let data = storedData ? JSON.parse(storedData) : [];
  const fireCommand = async param => {
    let [command, name, ...params] = param.split(":");
    if (!name) findData(command);
    else if (command.trim() == "modules") {
      findData("modules:");
    } else if (command.trim() == "new" && name) {
      createNewModule(name.trim());
    }
  };
  const findData = async param => {
    try {
      data = [];
      var descriptions = await fetch(`/api/search/${param || "nothing"}`);
      data = await descriptions.json();
      sessionStorage.setItem("search-results", JSON.stringify(data));
    } catch (error) {
      console.log(error);
    }
  };
  const createNewModule = async name => {
    try {
      var request = await fetch("/api/modules/" + name, {
        method: "POST"
      });
      let descriptors = await request.json();
      data = descriptors;

      setTimeout(() => {
        window.location.href = `/index.html?path=editor&module=${name}`;
      }, 500);
    } catch (error) {
      console.log(error);
    }
  };
  let keypress = event => {
    if (event.key === "Enter") {
      fireCommand(event.target.value);
    }
  };
</script>

<style>

</style>

<div class="content--center">
  <h1 class="title">Welcome to ZDragon!</h1>
  <p>
    Visit
    <a href="https://zdragon.nl" target="_blank" rel="noopener noreferrer">
      ZDragon.nl
    </a>
    to learn more about zdragon!
  </p>

  <div>
    <h2>Search your models:</h2>
    <input
      class="search"
      autocomplete="off"
      type="text"
      on:keypress={keypress} />
  </div>

  {#if data && data.length > 0}
    {#each data as d}
      <SearchResult descriptor={d} />
    {/each}
  {:else}
    <div>Your query returned no results.</div>
  {/if}
</div>
<!-- <HomeHelp /> -->
