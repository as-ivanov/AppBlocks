using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Roslyn.Common;
using MetricsCollector.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.Roslyn.MetricsCollector
{
  public class MetricsCollectorClassGenerator : InterfaceImplementationGenerator<MetricsCollectorDescriptor>
  {
    private const string LoggerFieldName = "_metricsProvider";
    public MetricsCollectorClassGenerator(AttributeData attributeData) : base(attributeData, new Version(1, 0, 0))
    {
    }

    protected override MetricsCollectorDescriptor GetImplementationDescriptor(TypeDeclarationSyntax typeDeclaration,
	    TransformationContext context, AttributeData attributeData)
    {
	    return typeDeclaration.ToMetricsCollectorDescriptor(context, attributeData);
    }

    protected override string[] GetNamespaces()
    {
	    return new []{ typeof(IMetricsProvider).Namespace };
    }

    protected override MemberDeclarationSyntax[] GetFields(MetricsCollectorDescriptor loggerDescriptor)
    {
	    throw new NotImplementedException();
    }

    protected override ConstructorDeclarationSyntax[] GetConstructors(MetricsCollectorDescriptor loggerDescriptor)
    {
	    throw new NotImplementedException();
    }

    protected override MemberDeclarationSyntax[] GetMethods(MetricsCollectorDescriptor loggerDescriptor)
    {
	    throw new NotImplementedException();
    }


    // private static MemberDeclarationSyntax GenerateLoggerImplementation(MetricsCollectorDescriptor descriptor,
    //   string className)
    // {
    //   var publicKeywordToken = Token(SyntaxKind.PublicKeyword);
    //
    //   var members = new List<MemberDeclarationSyntax>();
    //   foreach (var method in descriptor.Methods)
    //   {
    //     var methodParameters = method.Parameters.Select(CreateParameter).ToArray();
    //     var methodDeclaration = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), method.Identifier)
    //       .WithTypeParameterList(method.TypeParameterList)
    //       .WithConstraintClauses(method.ConstraintClauses)
    //       .AddModifiers(publicKeywordToken)
    //       .AddParameterListParameters(methodParameters)
    //       .WithBody(Block(GetMethodBody(method)));
    //     members.Add(methodDeclaration);
    //   }
    //
    //   var classDeclaration = CreateClass(descriptor, className, members.ToArray());
    //   return classDeclaration;
    // }

    // private static SyntaxList<StatementSyntax> GetMethodBody(MetricsCollectorMethod method)
    // {
    //   return SingletonList<StatementSyntax>(
    //     IfStatement(
    //       InvocationExpression(
    //           MemberAccessExpression(
    //             SyntaxKind.SimpleMemberAccessExpression,
    //             IdentifierName(LoggerFieldName),
    //             IdentifierName("IsEnabled")))
    //         .WithArgumentList(
    //           ArgumentList()),
    //       Block(
    //         SingletonList<StatementSyntax>(
    //           ExpressionStatement(
    //             InvocationExpression(
    //                 IdentifierName("_" + method.Identifier.ToCamelCase()))
    //               .WithArgumentList(GetCallArgumentList(method)))))));
    // }

    // private static ArgumentListSyntax GetCallArgumentList(MetricsCollectorMethod method)
    // {
    //   var arguments = new List<SyntaxNodeOrToken> { Argument(IdentifierName(LoggerFieldName)) };
    //   foreach (var parameter in method.Parameters)
    //   {
    //     if (arguments.Any())
    //     {
    //       arguments.Add(Token(SyntaxKind.CommaToken));
    //     }
    //
    //     arguments.Add(Argument(IdentifierName(parameter.Identifier.ToCamelCase())));
    //   }
    //
    //   var lastParameter = method.Parameters.LastOrDefault();
    //
    //   return ArgumentList(SeparatedList<ArgumentSyntax>(arguments));
    // }


    // private static ParameterSyntax CreateParameter(MetricsCollectorMethodParameter entry)
    // {
    //   var typeSyntax = entry.TypeSyntax;
    //   SyntaxList<AttributeListSyntax> attributes =
    //     new SyntaxList<AttributeListSyntax>(Array.Empty<AttributeListSyntax>());
    //   return Parameter(attributes, entry.Modifiers, typeSyntax, entry.Identifier, null);
    // }

    private static MemberDeclarationSyntax[] GenerateGeneralFields(MetricsCollectorDescriptor descriptor)
    {
      return new MemberDeclarationSyntax[]
      {
         FieldDeclaration(
             VariableDeclaration(
                 IdentifierName(typeof(IMetricsProvider).FullName))
               .WithVariables(
                 SingletonSeparatedList<VariableDeclaratorSyntax>(
                   VariableDeclarator(
                     Identifier(LoggerFieldName)))))
           .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
      };
    }

    private static ConstructorDeclarationSyntax GenerateConstructor(MetricsCollectorDescriptor descriptor,
      string className)
    {
      var constructorDeclaration = ConstructorDeclaration(
          Identifier(className))
        .WithModifiers(
          TokenList(
            Token(SyntaxKind.PublicKeyword)));

      return constructorDeclaration;
    }

    // private static ClassDeclarationSyntax CreateClass(MetricsCollectorDescriptor descriptor, string className,
    //   MemberDeclarationSyntax[] members)
    // {
    //   var classDeclaration = ClassDeclaration(className).WithModifiers(
    //       TokenList(Token(SyntaxKind.PublicKeyword)))
    //     .WithBaseList(GenerateBaseList(descriptor))
    //     .AddMembers(GenerateGeneralFields(descriptor))
    //     .AddMembers(GenerateLoggerFields(descriptor))
    //     .AddMembers(GenerateConstructor(descriptor, className))
    //     .AddMembers(members)
    //     .NormalizeWhitespace();
    //
    //   return classDeclaration;
    // }

    private static BaseListSyntax GenerateBaseList(MetricsCollectorDescriptor descriptor)
    {
      var syntaxNodeOrTokenList = new List<SyntaxNodeOrToken>();

      // if (descriptor.BaseClass != null)
      // {
      //   //Debugger.Launch();
      //   syntaxNodeOrTokenList.AddRange(
      //     new SyntaxNodeOrToken[]
      //     {
      //        SimpleBaseType(
      //          IdentifierName(descriptor.BaseClass)),
      //        Token(SyntaxKind.CommaToken)
      //     });
      // }

      syntaxNodeOrTokenList.AddRange(
        new SyntaxNodeOrToken[]
        {
           SimpleBaseType(
             IdentifierName(descriptor.DeclarationSyntax.Identifier)),
           Token(SyntaxKind.CommaToken),
           SimpleBaseType(
             IdentifierName("MetricsCollectorBase")),
           Token(SyntaxKind.CommaToken),
           SimpleBaseType(
             IdentifierName("ISingletonDependency"))
        });

      return BaseList(SeparatedList<BaseTypeSyntax>(syntaxNodeOrTokenList.ToArray()));
    }

    // private static MemberDeclarationSyntax[] GenerateLoggerFields(MetricsCollectorDescriptor descriptor)
    // {
    //   var result = new List<MemberDeclarationSyntax>();
    //   for (var index = 0; index < descriptor.Methods.Length; index++)
    //   {
    //     var method = descriptor.Methods[index];
    //
    //     var list = new List<SyntaxNodeOrToken>();
    //     foreach (var methodParameter in method.Parameters)
    //     {
    //       if (list.Count > 0)
    //       {
    //         list.Add(Token(SyntaxKind.CommaToken));
    //       }
    //
    //       list.Add(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(methodParameter.Identifier.Text)));
    //     }
    //
    //     var arrayInitializerExpression = InitializerExpression(SyntaxKind.ArrayInitializerExpression,
    //       SeparatedList<ExpressionSyntax>(list));
    //
    //     var declaration = FieldDeclaration(
    //         VariableDeclaration(
    //             ArrayType(
    //                 PredefinedType(
    //                   Token(SyntaxKind.StringKeyword)))
    //               .WithRankSpecifiers(
    //                 SingletonList<ArrayRankSpecifierSyntax>(
    //                   ArrayRankSpecifier(
    //                     SingletonSeparatedList<ExpressionSyntax>(
    //                       OmittedArraySizeExpression())))))
    //           .WithVariables(
    //             SingletonSeparatedList<VariableDeclaratorSyntax>(
    //               VariableDeclarator(
    //                   Identifier($"_{method.Identifier.ToCamelCase()}Keys"))
    //                 .WithInitializer(
    //                   EqualsValueClause(
    //                     arrayInitializerExpression)))))
    //       .WithModifiers(
    //         TokenList(
    //           new[]
    //           {
    //              Token(SyntaxKind.PrivateKeyword),
    //              Token(SyntaxKind.StaticKeyword),
    //              Token(SyntaxKind.ReadOnlyKeyword)
    //           }));
    //
    //     result.Add(declaration);
    //   }
    //
    //   return result.ToArray();
    // }

    private static string EscapeString(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return "\"\"";
      }

      return "@\"" + input.Replace("\"", "\"\"") + "\"";
    }

    private static string GetClassName(in SyntaxToken syntaxToken)
    {
      if (syntaxToken.Text.StartsWith("I"))
        return syntaxToken.Text.Substring(1);
      else
        return syntaxToken.Text;
    }
  }
}