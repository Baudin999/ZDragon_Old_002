import navigator from "./../navigator.js";

export let getCode = async moduleName => {
  var codeRequest = await fetch("/api/module/" + navigator.module);
  return await codeRequest.text();
};
