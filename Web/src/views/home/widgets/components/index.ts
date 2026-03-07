import { markRaw } from 'vue';
// Define component type
type Component = any;

const resultComps: Record<string, Component> = {};

// Array of component names to exclude
const excludeComponents = ['scheduleEdit'];

// Use import.meta.glob to dynamically import all .vue files in the current directory, eager import
const requireComponent = import.meta.glob('./*.vue', { eager: true });
// console.log(requireComponent);

Object.keys(requireComponent).forEach((fileName: string) => {
	// Process the file name, remove the leading './' and the trailing file extension
	const componentName = fileName.replace(/^\.\/(.*)\.\w+$/, '$1');

	// Skip import if component name is in exclude array
	if (excludeComponents.includes(componentName)) {
		return;
	}

	// Make sure the module export exists and is the default export
	const componentModule = requireComponent[fileName] as { default: Component };

	// Add component to resultComps, using processed filename as key
	resultComps[componentName] = componentModule.default;
});

// Mark resultComps as a primitive object to avoid making it reactive
export default markRaw(resultComps);
