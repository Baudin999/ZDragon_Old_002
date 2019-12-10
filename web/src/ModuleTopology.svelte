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
  var getData = async () => {
    var response = await fetch("/api/topology");
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
  getData();
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
</style>

<div class="topology">
  <div id="topology" />
</div>
