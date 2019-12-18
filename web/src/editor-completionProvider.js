export const completionProvider = (monaco, editor) => {
  monaco.languages.registerCompletionItemProvider("carlang", {
    provideCompletionItems: function(model, position) {
      var textUntilPosition = model.getValueInRange({
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: position.lineNumber,
        endColumn: position.column
      });
      var word = model.getWordUntilPosition(position);
      var range = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn
      };
      return {
        suggestions: [
          {
            label: "type_simple",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a type snippet",
            insertText: "@The ${1:Name} type description\ntype ${1:Name}",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "type_complex",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a type snippet",
            insertText:
              "@The ${1:Name} type description\ntype ${1:Name} =\n\t${2:FieldName}: ${3:FieldType};",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "field_simple",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a type snippet",
            insertText: "${1:FieldName}: ${2:FieldType};",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "alias",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create an alias",
            insertText: "alias ${1:Name} = ${2:Type};",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "choice_string",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a Choice",
            insertText:
              '@The ${1:Name} choice\nchoice ${1:Name} =\n\t| "${2:Option1}"\n\t| "${3:Option1}"',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "choice_number",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a Numbered Choice",
            insertText: "choice ${1:Name} =\n\t| 1\n\t| 2",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          {
            label: "view",
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "Create a Numbered Choice",
            insertText: "view ${1:Name} =\n\t${2:RootElement}",
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            range: range
          },
          ...BaseTypes(monaco, range)
        ]
      };
    }
  });
};

const BaseTypes = (monaco, range) => [
  {
    label: "String",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "The String base type",
    insertText: "String"
  },
  {
    label: "Number",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "Represents whol numbers",
    insertText: "Number"
  },
  {
    label: "Decimal",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "Represents decimal numbers",
    insertText: "Decimal"
  },
  {
    label: "Date",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "Represents the date without time",
    insertText: "Date"
  },
  {
    label: "Time",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "Represents the time",
    insertText: "Time"
  },
  {
    label: "DateTime",
    kind: monaco.languages.CompletionItemKind.Class,
    documentation: "Represents a date-time value",
    insertText: "DateTime"
  }
];
