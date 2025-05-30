document.addEventListener('DOMContentLoaded', () => {
    const themeSwitch = document.createElement('button');
    themeSwitch.className = 'theme-switch';
    themeSwitch.innerHTML = '🌓';
    document.body.appendChild(themeSwitch);

    // Перевіряємо збережену тему
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);

    // Функція для зміни теми
    const toggleTheme = () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        
        // Додаємо анімацію
        document.body.classList.add('fade-in');
        setTimeout(() => {
            document.body.classList.remove('fade-in');
        }, 300);
    };

    // Додаємо обробник подій
    themeSwitch.addEventListener('click', toggleTheme);

    // Перевіряємо системні налаштування теми
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        if (!localStorage.getItem('theme')) {
            document.documentElement.setAttribute('data-theme', 'dark');
            localStorage.setItem('theme', 'dark');
        }
    }

    // Слухаємо зміни системної теми
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
        if (!localStorage.getItem('theme')) {
            const newTheme = e.matches ? 'dark' : 'light';
            document.documentElement.setAttribute('data-theme', newTheme);
        }
    });
}); 