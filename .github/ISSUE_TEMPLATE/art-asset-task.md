---
name: Art Asset Task
about: Default template for generating art tasks in the TrenchRun project
title: "[ART] New Asset"
labels: ''
assignees: ''

---
## Asset Logistics
### **Asset Type:** 
  - *Examples: "Model", "Texture", "Video", etc. to describe what the asset IS*
  
### **Category:** 
  - *Examples: "Environment", "Enemy", "Power-Up", "UI", etc. to relate to Use Case below*

## Description


## Use Case(s)


## Steps
- [ ] Model(s)
  - [ ] Create .blend files in `~/Workflow/[Asset Type]/`
- [ ] UV Unwrapping
- [ ] Texturing
- [ ] Animation
  - [ ] Rig Made
  - [ ] Actions animated
- [ ] Export to FBX
  - Check Axis assignments
  - Ensure units are applied to model
  - If in doubt, use the "Flax and Unity" presets
- [ ] Import to Flax
  - [ ] Model set up - materials & textures
  - [ ] Animation Graph set up
- [ ] Prefab(s) made (if applicable)
