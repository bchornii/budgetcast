## Pull Request Checklist

### General
- [ ] Code performs the intended functionality and meets acceptance criteria.
- [ ] Scope of the PR is limited to the linked story/issue.
- [ ] PR links to the correct work item or issue.
- [ ] PR title includes the Jira issue key (e.g., `APP-123`) [Optional, for Jira only]
- [ ] Code is clear, readable, and self-explanatory (naming, structure, simplicity).
- [ ] Code follows team/project coding conventions (formatting, style, linting).
- [ ] No redundant, duplicate, or dead code is present.
- [ ] Code is modular, loosely coupled, and adheres to SOLID principles.
- [ ] No excessive complexity (methods/classes are reasonably sized).
- [ ] No unnecessary global/static state is introduced.
- [ ] Proper error handling and logging are implemented.
- [ ] Debugging or temporary code is removed.

### Security
- [ ] All inputs are validated (type, length, format, range).
- [ ] Output is properly encoded/escaped to prevent injection attacks.
- [ ] Sensitive data (passwords, tokens, PII) is never logged or hardcoded.
- [ ] Authentication, authorization, and access controls are respected.
- [ ] External libraries are from trusted sources and pinned to safe versions.
- [ ] Exceptions and errors do not expose sensitive information.

### Testing
- [ ] Code is testable (dependencies are injectable, interfaces are clear).
- [ ] Unit tests exist for critical logic with agreed coverage.
- [ ] Integration tests cover interactions with external systems.
- [ ] Edge cases, error handling, and boundary conditions are tested.
- [ ] Tests run automatically and pass locally and in CI.
- [ ] Test data does not contain secrets or sensitive information.

### Documentation
- [ ] Public methods, classes, and APIs are documented.
- [ ] Comments explain the “why” (intent) rather than the “what.”
- [ ] README / usage guides are updated if necessary.
- [ ] Any non-obvious design decisions are explained in code comments or docs.
- [ ] TODOs or unfinished code are flagged and justified.

### Performance & Scalability
- [ ] Code avoids unnecessary computations, memory usage, or DB calls.
- [ ] Loops, queries, and recursive calls are efficient.
- [ ] No premature optimization — but bottlenecks are considered.
- [ ] Logging is efficient and does not flood output in production.
- [ ] External service calls are retried/backed off responsibly.

### Architecture & Design
- [ ] Code fits into the overall system architecture and design principles.
- [ ] Dependencies are appropriate and minimal.
- [ ] APIs follow established patterns (REST, GraphQL, RPC, etc.).
- [ ] Feature toggles, flags, or configuration are used where needed.
- [ ] Code changes do not introduce tight coupling or hidden side effects.
