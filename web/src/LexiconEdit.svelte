<script>
  import navigator from "./navigator.js";
  let data = {
    tags: [],
    applications: []
  };
  let config = {
    functionalOwners: [],
    technicalOwners: [],
    domains: []
  };
  let getConfiguration = async () => {
    var response = await fetch("/api/lexicon/config");
    config = await response.json();
  };
  getConfiguration();
  let fetchData = async params => {
    if (!params || params.length === 0) {
      data = null;
      return;
    }
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

</style>

{#if !data}
  <div>Could not find your lexicon entry.</div>
{:else}

  <div class="content--center">
    <h1 class="title">Edit your Lexicon Entry!</h1>
    <span class="nav-button" on:click={submit}>Save</span>
    <span class="nav-button" on:click={() => navigator.navigate('lexicon')}>
      Cancel
    </span>
    <div class="edit-form">

      <form submit="() => ">
        <div class="left">
          <div>
            <label>Domain:</label>
            <select bind:value={data.domain}>
              <option />
              {#each config.domains as domain}
                <option>{domain}</option>
              {/each}
            </select>
          </div>
          <div>
            <label>Name:</label>
            <input bind:value={data.name} />
          </div>
          <div>
            <label>Description:</label>
            <textarea bind:value={data.description} />
          </div>
        </div>
        <div class="right">
          <div>
            <label>Functional Owner:</label>
            <select bind:value={data.functionalOwner}>
              <option />
              {#each config.functionalOwners as owner}
                <option>{owner}</option>
              {/each}
            </select>
          </div>
          <div>
            <label>Technical Owner:</label>
            <select bind:value={data.technicalOwner}>
              <option />
              {#each config.technicalOwners as owner}
                <option>{owner}</option>
              {/each}
            </select>
          </div>

          <h2>Tag your entry</h2>
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
    </div>
  </div>
{/if}
