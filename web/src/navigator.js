import { writable } from "svelte/store";

class Navigator {
  constructor() {
    this.route = writable("index");
  }
  navigate(route) {
    this.route.update(n => route);
  }

  subscribe(f) {
    this.route.subscribe(f);
  }
}

export default new Navigator();
