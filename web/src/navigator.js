import { writable } from "svelte/store";

class Navigator {
  constructor() {
    this.route = writable(getUrlParameter("path") || "index");
    this.params = writable([]);
    this.module = getUrlParameter("module") || null;
  }
  navigate(route, ...params) {
    this.route.update(n => route);
    this.params.update(n => params || []);
    UpdateQueryString("path", route);
  }

  subscribe(f) {
    this.route.subscribe(f);
  }

  $params(f) {
    this.params.subscribe(f);
  }
}

export default new Navigator();

function UpdateQueryString(key, value, url) {
  if (!url) url = window.location.href;
  var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi"),
    hash;

  if (re.test(url)) {
    if (typeof value !== "undefined" && value !== null) {
      var _url = url.replace(re, "$1" + key + "=" + value + "$2$3");
      window.location.href = _url;
    } else {
      hash = url.split("#");
      url = hash[0].replace(re, "$1$3").replace(/(&|\?)$/, "");
      if (typeof hash[1] !== "undefined" && hash[1] !== null) {
        url += "#" + hash[1];
      }
      window.location.href = url;
      return url;
    }
  } else {
    if (typeof value !== "undefined" && value !== null) {
      var separator = url.indexOf("?") !== -1 ? "&" : "?";
      hash = url.split("#");
      url = hash[0] + separator + key + "=" + value;
      if (typeof hash[1] !== "undefined" && hash[1] !== null) {
        url += "#" + hash[1];
      }
      window.location.href = url;
      return url;
    } else {
      return url;
    }
  }
}
function getUrlParameter(name) {
  name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
  var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
  var results = regex.exec(location.search);
  return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
