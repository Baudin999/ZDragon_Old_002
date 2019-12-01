<script>
  import navigator from "./navigator.js";
  let data = {};
  let submit = async () => {
    await fetch("https://localhost:5001/api/lexicon", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    });
    navigator.navigate("lexicon");
  };
</script>

<style>
  h1 {
    color: #ff3e00;
    text-transform: uppercase;
    font-size: 4em;
    font-weight: 100;
  }

  input,
  textarea {
    width: 450px;
  }
  textarea {
    height: 250px;
  }
</style>

<h1>Please describe something</h1>

<div>
  <form>
    <div>
      <label>Name:</label>
      <input
        on:change={e => {
          data.name = e.target.value;
        }} />
    </div>
    <div>
      <label>Description:</label>
      <textarea
        on:change={e => {
          data.description = e.target.value;
        }} />
    </div>
    <button type="button" on:click={submit}>Add</button>
  </form>
</div>
