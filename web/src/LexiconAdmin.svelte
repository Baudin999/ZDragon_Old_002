<script>
  //   import { onMount } from "svelte";

  //   onMount(() => {
  //     flask = new CodeFlask("#domain-container", { language: "js" });
  //     flask = new CodeFlask("#owner-container", { language: "js" });
  //   });
</script>

<style>
  textarea {
    width: 750px;
    min-height: 250px;
  }
</style>

<h2>Domains</h2>
<p>Please give the domains you would like to restrict your lexicon to.</p>
<textarea />

<h2>Owners</h2>
<p>Please give the owners you would like to restrict the lexicon to.</p>
<textarea />
