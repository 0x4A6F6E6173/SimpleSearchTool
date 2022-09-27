# General structure

The public interface is provided as a set of static functions found in the *Search* class. The main function of the interface is **WildcardSearch** which receives a Query and a Solver.

Any viable solver inherits from the IWildcardSolver interface