<script>
  export let descriptor;

  let selectNode = descriptor => {
    //
  };
</script>

<style>
  .descriptor {
    border: 1px solid lightgray;
    font-size: 14px;
    padding: 1;
    margin: 0;
    width: 350px;
    margin-bottom: 1rem;
    margin-left: 50%;
    transform: translateX(-50%);
  }
  .descriptor h2 {
    background: #174372;
    color: white;
    padding: 0.5em;
    font-size: 1em;
    margin: 0;
  }
  .descriptor .description {
    color: gray;
    padding: 0 1rem;
  }
</style>

<div class="descriptor" on:click={() => selectNode(descriptor)}>
  <h2>
    {descriptor.module} - {descriptor.parent ? descriptor.parent + '.' : ''}{descriptor.name}
  </h2>
  <p class="description">{descriptor.description || 'No Description'}</p>
  <a alt={descriptor.module} href={`/${descriptor.module}/index.html`}>
    Module: {descriptor.module}
  </a>
  {#if !descriptor.parent}
    <br />
    <a
      alt={descriptor.name}
      href={`/api/data/${descriptor.module}/${descriptor.name}`}>
      Show me the data!
    </a>
  {/if}
</div>
