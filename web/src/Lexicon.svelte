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
    // TODO: add logic to detect admin user.
    alert("Only administrators are allowed to delete lexicon items.");
    if (true) return;

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

  findData("all:");
</script>

<style>
  tr:hover {
    cursor: pointer;
  }
</style>

<div class="content--center">
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
      class="search"
      type="text"
      on:keyup={e => e.code === 'Enter' && onkeyup(e.target.value)}
      on:change={e => findData(e.target.value)} />
  </div>
</div>
<table>
  <thead>
    <th>Domain</th>
    <th>Name</th>
    <th>Description</th>
  </thead>
  <tbody>
    {#each data as d}
      <tr on:click={() => navigator.navigate('edit-lexicon', d.id)}>
        <td>{d.domain}</td>
        <td>{d.name}</td>
        <td>{d.description}</td>
      </tr>
    {/each}
  </tbody>
</table>
