# The language

ZDragon models are written in a language called `car-lang`. This is a combination of `markdown` and
a custom language. You can use this language to model your data and your processes.

It's always good to get the terminology right. ZDragon is the tool, this tool has a CLI and can
build/watch and serve your content. ZDragon also parses your `car-lang` code and transpiles it to
HTML, XSDs and JSON schemas.

## Types

Types are the bread and butter of ZDragon and explain how certain data structures should look. Type
are easily created, like so:

```
type Person
```

This is a simple type and for all intents and purposes this is all you will need to define types.
Because ZDragon is a language which compiles to different formats we can start the web server and
look at the resulting web page. The page will contain a diagram (rendered in
[Mermaidjs](https://mermaidjs.github.io/#/)) and a table describing the type.

![](/assets/01.png)

You can add fields to your type:

```
type Person =
    FirstName: String;
    LastName: String;
```

`car-lang` is a mostly indentation based language but with terminal statements `;`. What this means
is that `car-lang` sometimes needs you to end your sentences with a `;` and sometimes, mostly with
root objects, you do not need to.

## Relationships

ZDragon is smart enough to recognize relationships between entities.

```
type Person =
    FirstName: String;
    LastName: String;
    Address: Address;

type Address =
    Street: String;
    HouseNumber: Number;
```

![](/assets/02.png)

## Cardinality

```
type Person =
    FirstName: String;
    LastName: String;
    Address: List Address;

type Address =
    Street: String;
    HouseNumber: Number;
```

![](/assets/03.png)
