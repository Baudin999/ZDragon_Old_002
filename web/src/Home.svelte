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
  .result-list {
    overflow: auto;
    height: 400px;
  }
  .logo {
    /*position: fixed;
    top: 0;
    left: 0; 
    width: 100%;
    height: 100%;
    max-height: 100%;
    background-image: url("/logo.jpg");
    -webkit-background-size: cover;
    -moz-background-size: cover;
    -o-background-size: cover;
    background-size: cover;
    opacity: 0.6;
    z-index: -2; */
  }
</style>

<div class="logo" />
<div class="content--center">
  <h1 class="title">Welcome to ZDragon!</h1>

  <div>
    <h2>Search your models:</h2>
    <input
      class="search"
      autocomplete="off"
      type="text"
      on:keypress={keypress} />
  </div>

  <div class="result-list">
    {#if data && data.length > 0}
      {#each data as d}
        <SearchResult descriptor={d} />
      {/each}
    {:else}
      <div>Your query returned no results.</div>
    {/if}
  </div>
</div>
<!-- <HomeHelp /> -->
