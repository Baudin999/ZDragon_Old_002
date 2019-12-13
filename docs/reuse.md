# Reuse

Reuse is like a double edged sword in that it can give you real benefits but at the cost of
complexity. We, your resident ZDragoners, advice against a lot of reuse. We understand that this is
a controversial topic and not something we are willing to enforce by not supporting resue but keep
in mind that this will make your modelling harder the more you use these features.

## Extension

One way of reusing a definition is by extending a type with the fields and context of another type.
You can use extension for this:

```
type Person =
    FirstName: String;

type Customer extends Person
```

The type `Customer` will now also have `FirstName` field with exactly the same charictaristics as
the FirstName field of the Person.

If for some reason you would like to override a subset of the fields you get through extending the
type you can simply add a new version of that field to your type.

```
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Customer extends Person =
    LastName: String
        & min 2
        & max 40;
```
