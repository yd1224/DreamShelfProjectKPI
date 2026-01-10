export function useScroll (){
    const scrollPosition = ref(0);

    function lockScroll() {
        scrollPosition.value = window.scrollY;
        document.body.style.position = 'fixed';
        document.body.style.top = `-${scrollPosition.value}px`;
        document.body.style.left = '0';
        document.body.style.right = '0';
    }

    function unlockScroll() {
        document.body.style.position = '';
        document.body.style.top = '';
        document.body.style.left = '';
        document.body.style.right = '';
        window.scrollTo(0, scrollPosition.value);
    }

    return {
        lockScroll,
        unlockScroll
    }
}