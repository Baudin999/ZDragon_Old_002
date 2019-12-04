<script>
  import navigator from "./navigator.js";
  let data = [];
  let search = "";
  let findData = async _search => {
    if (!_search) return;
    search = _search;
    var fetchResult = await fetch("/api/lexicon?query=" + search);
    var result = await fetchResult.json();
    data = result || [];
  };

  let deleteItem = async entry => {
    var result = await fetch("/api/lexicon", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(entry)
    });
    findData(search);
  };

  let onkeyup = query => {
    findData(query);
  };
</script>

<style>
  .items {
    display: block;
    width: 1024px;
    position: relative;
    left: 50%;
    transform: translateX(-50%);
  }
  .item {
    position: relative;
    display: flex;
    flex-direction: row;
    border: 1px solid lightgray;
    margin: 3px;
  }
  .item--name {
    flex: 150px;
    text-align: left;
    border-right: 1px solid lightgray;
    padding: 0.5em;
    font-weight: bold;
    text-decoration: underline;
  }
  .item--description {
    flex: 500px;
    text-align: justify;
    padding: 0.5em;
    padding-right: 3em;
  }
  .item--description__owner {
    margin-top: 0.5em;
    font-size: 0.7em;
  }
  .item__delete {
    color: red;
    font-weight: bolder;
    position: absolute;
    right: 10px;
    cursor: pointer;
    top: 50%;
    transform: translateY(-50%);
  }
</style>

<h1 class="title">Search your lexicon!</h1>

<div>
  <span
    class="nav-button"
    on:click={() => {
      navigator.navigate('add-lexicon');
    }}>
    Create
  </span>
  <h2>Search your lexicon:</h2>
  <input
    type="text"
    on:keyup={e => e.code === 'Enter' && onkeyup(e.target.value)}
    on:change={e => findData(e.target.value)} />
</div>

<div class="items">
  <div class="item">
    <div class="item--name">Domain</div>
    <div class="item--name">Name</div>
    <div class="item--description">Description</div>
  </div>
  {#each data as d}
    <div class="item">
      <div class="item--name">{d.domain}</div>
      <div
        class="item--name"
        on:click={() => navigator.navigate('edit-lexicon', d.id)}>
        {d.name}
      </div>
      <div class="item--description">
        {d.description}
        <div class="item--description__owner">Owner: {d.dataOwner}</div>
      </div>
      <span class="item__delete" on:click={() => deleteItem(d)}>X</span>
    </div>
  {/each}
</div>
