class Services {
  async getModules() {
    try {
      const response = await fetch("/api/modules");
      const modules = await response.json();
      return modules;
    } catch (error) {
      console.log(error);
    }
  }

  async search(value) {
    try {
      const response = await fetch("/api/search/" + value);
      return await response.json();
    } catch (error) {
      console.log(error);
    }
  }
}

class Renderers {
  renderModule(modules, root) {
    modules.forEach(module => {
      const div = document.createElement("div");
      div.innerText = module;
      root.appendChild(div);
    });
  }

  renderSearchResults(searchResults, root) {
    var template = searchResults
      .map(r => {
        return `
<div>
<p>${r.module}.${r.name}</p>
</div>
          `.trim();
      })
      .join("");
    root.innerHTML = template;
  }
}

function $(className) {
  return [...document.getElementsByClassName(className)][0];
}
