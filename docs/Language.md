# The language

ZDragon is written in a language called `car-lang`. This is a combination of `markdown` and a custom
language.

## Types

Types are the bread and butter of ZDragon and explain how certain data structures should look. Type
are easilly created, like so:

```
type Person
```

This is a simple type and for all intents and purposes this is all you will need to define types.
Because ZDragon is a language which compiles to different formats we can start the web server and
look at the resulting web page. The page will contain a diagram (rendered in
[Mermaidjs](https://mermaidjs.github.io/#/)) and a table describing the type.

![](/assets/01.png)
