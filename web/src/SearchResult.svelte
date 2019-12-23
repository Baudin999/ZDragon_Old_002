<script>
  export let descriptor;

  let selectNode = async descriptor => {
    let url = `/api/svg?module=${descriptor.module}&name=${descriptor.name}&type=g`;
    let _fetchResult = await fetch(url);
    let result = await _fetchResult.text();

    var element = document.getElementById(id);
    mermaid.render(
      "inner-" + id,
      result,
      (svg, bind) => {
        {
          element.innerHTML = svg;
        }
      },
      element
    );
  };
  let id =
    "mermaid-" +
    Math.random()
      .toString(36)
      .replace(/[^a-z]+/g, "")
      .substr(2, 10) +
    "-" +
    Date.now();
</script>

<style>
  .descriptor {
    border: 1px solid lightgray;
    font-size: 14px;
    padding: 1;
    margin: 0;
    width: 450px;
    margin-bottom: 1rem;
    margin-left: 50%;
    transform: translateX(-50%);
    background: white;
  }
  .descriptor:hover {
    cursor: pointer;
  }
  .descriptor h2 {
    background: #3083db;
    color: white;
    padding: 0.5em;
    font-size: 1em;
    margin: 0;
    position: relative;
  }
  .descriptor .description {
    color: gray;
    padding: 0 1rem;
  }
  .pill {
    background: orange;
    border: 1 px solid rgb(172, 114, 6);
    color: white;
    border-radius: 50%;
    font-size: 10px;
    text-transform: lowercase;
    padding: 0.5em 1em 0.6em 1em;
    position: absolute;
    left: 10px;
    top: 50%;
    transform: translateY(-50%);
  }
  .pill.type {
    background: purple;
  }
  .pill.field {
    background: darkgreen;
  }
</style>

<div class="descriptor" on:click={() => selectNode(descriptor)}>
  <h2>
    <span class="pill {descriptor.descriptorType.toLowerCase()}">
      {descriptor.descriptorType}
    </span>
    {descriptor.module} - {descriptor.parent ? descriptor.parent + '.' : ''}{descriptor.name}
  </h2>
  <p class="description">{descriptor.description || 'No Description'}</p>
  <a alt={descriptor.module} href={`?path=preview&module=${descriptor.module}`}>
    Module: {descriptor.module}
  </a>
  {#if descriptor.descriptorType === 'Type'}
    <br />
    <a
      alt={descriptor.name}
      href={`/api/data/${descriptor.module}/${descriptor.name}`}>
      Show me the data!
    </a>
  {/if}

  <div {id} />
</div>
