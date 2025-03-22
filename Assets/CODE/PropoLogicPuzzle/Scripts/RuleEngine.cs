using System;
using System.Collections.Generic;

public static class RuleEngine
{
    /// <summary>
    /// Attempts to match a pattern against a node.
    /// Pattern variables are assumed to be VariableNodes with lower–case names.
    /// </summary>
    public static bool TryMatch(ExpressionNode node, ExpressionNode pattern, Dictionary<string, ExpressionNode> subs)
    {
        if (pattern is VariableNode pVar && char.IsLower(pVar.variable[0]))
        {
            // Pattern variable: if not substituted yet, assign it.
            if (!subs.ContainsKey(pVar.variable))
            {
                subs[pVar.variable] = node;
                return true;
            }
            else
            {
                // Otherwise, the node must equal the previously assigned node.
                return AreEqual(node, subs[pVar.variable]);
            }
        }
        else if (pattern is ConstantNode pConst && node is ConstantNode nodeConst)
        {
            return pConst.value == nodeConst.value;
        }
        else if (pattern is VariableNode pVar2 && node is VariableNode nodeVar)
        {
            // For non-placeholder variables (assumed to be upper–case), they must match exactly.
            return pVar2.variable == nodeVar.variable;
        }
        else if (pattern is OperatorNode pOp && node is OperatorNode nodeOp)
        {
            if (pOp.op != nodeOp.op)
                return false;
            if (pOp.operands.Count != nodeOp.operands.Count)
                return false;
            for (int i = 0; i < pOp.operands.Count; i++)
            {
                if (!TryMatch(nodeOp.operands[i], pOp.operands[i], subs))
                    return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks structural equality of two expression trees.
    /// </summary>
    public static bool AreEqual(ExpressionNode a, ExpressionNode b)
    {
        if (a is VariableNode va && b is VariableNode vb)
            return va.variable == vb.variable;
        if (a is ConstantNode ca && b is ConstantNode cb)
            return ca.value == cb.value;
        if (a is OperatorNode oa && b is OperatorNode ob)
        {
            if (oa.op != ob.op || oa.operands.Count != ob.operands.Count)
                return false;
            for (int i = 0; i < oa.operands.Count; i++)
                if (!AreEqual(oa.operands[i], ob.operands[i]))
                    return false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Replaces pattern variables in a template with their corresponding substitutions.
    /// </summary>
    public static ExpressionNode Substitute(ExpressionNode template, Dictionary<string, ExpressionNode> subs)
    {
        if (template is VariableNode varNode && char.IsLower(varNode.variable[0]))
        {
            if (subs.ContainsKey(varNode.variable))
                return subs[varNode.variable];
            else
                return template;
        }
        else if (template is OperatorNode opNode)
        {
            List<ExpressionNode> newOperands = new List<ExpressionNode>();
            foreach (var operand in opNode.operands)
            {
                newOperands.Add(Substitute(operand, subs));
            }
            return new OperatorNode(opNode.op, newOperands.ToArray());
        }
        else
        {
            return template;
        }
    }

    /// <summary>
    /// Traverses the expression tree and attempts to apply the transformation rule.
    /// If a match is found, returns the transformed tree; otherwise, returns the original tree.
    /// </summary>
    public static ExpressionNode ApplyTransformation(ExpressionNode expression, ExpressionNode pattern, ExpressionNode replacement)
    {
        Dictionary<string, ExpressionNode> subs = new Dictionary<string, ExpressionNode>();
        if (TryMatch(expression, pattern, subs))
        {
            // Match found: build new tree from replacement template.
            return Substitute(replacement, subs);
        }
        // Otherwise, if expression is an operator node, try to transform its children.
        if (expression is OperatorNode opNode)
        {
            bool changed = false;
            List<ExpressionNode> newOperands = new List<ExpressionNode>();
            foreach (var operand in opNode.operands)
            {
                ExpressionNode newOperand = ApplyTransformation(operand, pattern, replacement);
                if (!AreEqual(newOperand, operand))
                    changed = true;
                newOperands.Add(newOperand);
            }
            if (changed)
                return new OperatorNode(opNode.op, newOperands.ToArray());
        }
        return expression;
    }
    
    /// <summary>
    /// Applies a transformation based on a rule asset.
    /// Returns the transformed expression if a match was found, or the original if not.
    /// </summary>
    public static ExpressionNode ApplyRule(ExpressionNode expression, string patternStr, string replacementStr)
    {
        LogicParser parser = new LogicParser();
        ExpressionNode pattern = parser.Parse(patternStr);
        ExpressionNode replacement = parser.Parse(replacementStr);
        return ApplyTransformation(expression, pattern, replacement);
    }
}
