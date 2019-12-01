<script>
  import navigator from "./navigator.js";
  let data = [];
  let findData = async search => {
    var fetchResult = await fetch("https://localhost:5001/api/lexicon");
    var result = await fetchResult.json();
    data = result;
  };

  let deleteItem = async entry => {
    var result = await fetch("https://localhost:5001/api/lexicon", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(entry)
    });
    findData();
  };

  findData();
</script>

<style>
  h1 {
    color: #ff3e00;
    text-transform: uppercase;
    font-size: 4em;
    font-weight: 100;
  }
  .items {
    display: block;
    width: 700px;
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
    flex: 200px;
    text-align: left;
    border-right: 1px solid lightgray;
    padding: 0.5em;
    font-weight: bold;
  }
  .item--description {
    flex: 500px;
    text-align: justify;
    padding: 0.5em;
    padding-right: 3em;
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

<h1>Search your lexicon!</h1>

<div>
  <span
    class="nav-button"
    on:click={() => {
      navigator.navigate('add-lexicon');
    }}>
    Create
  </span>
  <h2>Search your lexicon:</h2>
  <input type="text" on:change={e => findData(e.target.value)} />
</div>

<div class="items">
  {#each data as d}
    <div class="item">
      <div class="item--name">{d.name}</div>
      <div class="item--description">{d.description}</div>
      <span class="item__delete" on:click={() => deleteItem(d)}>X</span>
    </div>
  {/each}
</div>
