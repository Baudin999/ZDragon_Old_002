# Flows

Flows are the ZDragon way of creating data which flows.

## Simple Flow

```
flow "Do Something" =
    API -> Service :: Id -> String;
```

A flow begins by the keyword `flow` followed by either an `Identifier` or a `String`. Each flow
contains steps. A step is either:

1. From -> To
2. Composition
3. Loop

Composition and Loops end with a `;`.

## Composition

Composing steps means that calls are not sequential but results are passed to the next step.

```
flow "Get Students" =
    compose
        API -> Service :: Id -> Student;
        Service -> Database :: Id -> Student;
    ;
```

![](/assets/04.png)

## Loop

Flows also need to loop:

```
flow GetStudents =
    loop
        Service -> Service :: String
    ;
```
