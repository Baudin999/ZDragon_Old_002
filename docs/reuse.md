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

## Plucking Fields

The previous chapter described how to reuse entire types and extend these types with extra
information. Plucking fields is the other option for reuse. the idea is that you can pluck a single
field from a type.

```
type Person =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;
    DateOfBirth: Date;

type Customer =
    pluck Person.FirstName;
    pluck Person.LastName;
```

The `Customer` type will now only have the `FirstName` and the `LastName` exactly as defined in the
`Person` type. This is a fairly tedious but useful way of reusing your types.

## Breaking things up

Another way to achieve the previous result is to break types up into smaller types.

```
type Namable =
    FirstName: String
        & pattern /[A-Z][a-z]{2, 23}/
    ;
    LastName: String;

type Birthable =
    DateOfBirth: Date;

type Person extends Namable Birthable
type Customer extends Namable
```

The approach requires you to think about how types are broken up and how they will be used in your
processes and APIs. This approach is hard but eventually it will pay off. In this way you can break
up your types into subsets you can actually reuse in your running software.
