## Pull Request Checklist

### General
- [ ] Does the code work? (performs its intended function, logic is correct, etc.)
- [ ] Is the code easily understood?
- [ ] Does it conform to agreed coding conventions (naming, formatting, indentation, comments, etc.)?
- [ ] Is there any redundant or duplicate code?
- [ ] Is the code as modular as possible?
- [ ] Can any global variables be replaced?
- [ ] Is there any commented-out code?
- [ ] Do loops have a set length and correct termination conditions?
- [ ] Can any code be replaced with library functions?
- [ ] Can any logging or debugging code be removed?

### Security
- [ ] Are all data inputs checked (type, length, format, range) and encoded?
- [ ] Are errors from third-party utilities caught and handled?
- [ ] Are output values checked and encoded?
- [ ] Are invalid parameter values handled?

### Testing
- [ ] Is the code testable (dependencies manageable, test frameworks can access methods, etc.)?
- [ ] Do tests exist and are they comprehensive (meeting agreed coverage)?
- [ ] Do unit tests validate intended functionality?
- [ ] Are arrays checked for out-of-bound errors?
- [ ] Could any test code be replaced with existing APIs?

### Documentation
- [ ] Do comments exist and describe the intent of the code?
- [ ] Are all functions documented?
- [ ] Is unusual behavior or edge-case handling described?
- [ ] Are third-party library usage and purpose documented?
- [ ] Are data structures and units of measurement explained?
- [ ] Is any incomplete code flagged (e.g., with `TODO`) or removed?
