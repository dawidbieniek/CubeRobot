window.replaceParentPlaceholderContainer = (parentClassName, childClassName) => {
    const parentElements = document.querySelectorAll(`.${parentClassName}`);
    parentElements.forEach(parentElement => {
        const childDiv = parentElement.querySelector('div');
        if (childDiv) {
            childDiv.classList.add(...childClassName.split(' '));
            parentElement.parentNode.replaceChild(childDiv, parentElement);
        }
    });
};