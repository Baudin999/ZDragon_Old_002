<script>
  import navigator from "./navigator.js";
  let data = {
    tags: [],
    applications: []
  };
  let fetchData = async params => {
    var query = await fetch("/api/lexicon/" + params);
    var _data = await query.json();
    data = _data;
  };
  navigator.$params(fetchData);

  let newTag = "";
  let newApplication = "";
  let submit = async () => {
    if (!data.name || !data.domain || !data.description) return;
    await fetch("/api/lexicon", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    });
    navigator.navigate("lexicon");
  };

  let changeTag = e => {
    newTag = e.target.value;
  };
  let addTag = () => {
    data.tags = data.tags || [];
    if (newTag && newTag.length > 0 && data.tags.indexOf(newTag) === -1) {
      data.tags.push(newTag);
      newTag = "";
    }
  };
  let removeTag = tag => {
    data.tags = data.tags.filter(t => t !== tag);
  };

  let changeApplication = e => {
    newApplication = e.target.value;
  };
  let addApplication = () => {
    data.applications = data.applications || [];
    if (
      newApplication &&
      newApplication.length > 0 &&
      data.applications.indexOf(newApplication) === -1
    ) {
      data.applications.push(newApplication);
      newApplication = "";
    }
  };
  let removeApplication = tag => {
    data.applications = data.applications.filter(t => t !== tag);
  };

  let onkeyup = (event, f) => {
    if (event.code === "Enter") {
      if (f === "a") addApplication();
      else if (f === "t") addTag();
    }
  };
</script>

<style>
  input,
  textarea {
    max-width: 450px;
    width: 100%;
  }
  textarea {
    height: 250px;
  }

  form {
    display: flex;
    flex-direction: row;
  }
  label {
    font-weight: bold;
  }
  input,
  textarea,
  button {
    outline: none;
    border-radius: 0;
  }
  button:hover {
    cursor: pointer;
  }
  input:active,
  textarea:active {
    outline: none;
  }
  .left,
  .right {
    flex: 50%;
    margin: 0;
    padding: 1em;
    position: relative;
  }

  .delete {
    color: red;
  }
  .delete:hover {
    cursor: pointer;
  }

  @media (max-width: 750px) {
    form {
      flex-direction: column;
    }
  }
</style>

<h1 class="title">Edit your something</h1>

<div>
  <form>
    <div class="left">
      <div>
        <label>Domain:</label>
        <input
          value={data.domain}
          on:change={e => {
            data.domain = e.target.value;
          }} />
      </div>
      <div>
        <label>Name:</label>
        <input
          value={data.name}
          on:change={e => {
            data.name = e.target.value;
          }} />
      </div>
      <div>
        <label>Description:</label>
        <textarea
          value={data.description}
          on:change={e => {
            data.description = e.target.value;
          }} />
      </div>
      <div>
        <label>Data owner:</label>
        <input
          value={data.dataOwner}
          on:change={e => {
            data.dataOwner = e.target.value;
          }} />
      </div>
    </div>
    <div class="right">
      <div style="margin-bottom: 1em;">
        <label>Applications:</label>
        <div class="input">
          <input
            value={newApplication}
            on:change={changeApplication}
            on:keyup={e => onkeyup(e, 'a')} />
        </div>
        {#each data.applications as application}
          <div>
            {application}
            <span
              class="delete"
              on:click={() => removeApplication(application)}>
              X
            </span>
          </div>
        {/each}
      </div>

      <div>
        <label>Tags:</label>
        <div class="input">
          <input
            value={newTag}
            on:change={changeTag}
            on:keyup={e => onkeyup(e, 't')} />
        </div>
        {#each data.tags as tag}
          <div>
            {tag}
            <span class="delete" on:click={() => removeTag(tag)}>X</span>
          </div>
        {/each}
      </div>
    </div>
  </form>
  <button type="button" on:click={submit}>Save</button>
</div>
