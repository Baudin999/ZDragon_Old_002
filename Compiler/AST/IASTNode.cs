﻿using System;
namespace Compiler.AST
{
    public interface IASTNode {
        string Module { get; }
        Token? Token { get; }
    }
}
