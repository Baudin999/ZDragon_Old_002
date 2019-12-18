<script>
  import navigator from "./navigator.js";

  var options = {
    interaction: {
      dragNodes: false,
      hover: true
    },
    manipulation: {
      enabled: false
    }
  };
  var getData = async e => {
    var url = e ? "/api/topology" : "/api/topology/modules";
    var response = await fetch(url);
    var topology = await response.json();
    var container = document.getElementById("topology");
    var network = new vis.Network(container, topology, options);

    network.on("click", function(params) {
      let [n, ...rest] = params.nodes;
      let node = topology.nodes.find(node => node.id === n);
      if (node && node.module) {
        navigator.preview(node.module);
      }
    });
  };
  getData(false);
</script>

<style>
  .topology {
    height: calc(100% - 4rem);
  }
  #topology {
    height: 100%;
  }
  #topology > *:focus {
    outline: none !important;
  }
  .options-form {
    position: fixed;
    right: 10px;
    bottom: 10px;
  }
</style>

<div class="topology">
  <div id="topology" />
  <div class="options-form">
    <div class="form-field">
      <label>Include Details</label>
      <input
        type="checkbox"
        on:change={e => {
          getData(e.target.checked);
        }} />
    </div>
  </div>
</div>
