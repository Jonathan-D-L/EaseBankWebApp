//Back to top arrow
try {
    const arrow = document.getElementById('top-arrow')
    arrow.addEventListener('click', function () {
        window.scrollTo({ top: 0, behavior: "smooth" })
    })
    window.addEventListener('scroll', function () {
        const scrollPosition = window.scrollY
        if (scrollPosition >= 700) {
            arrow.style.opacity = '1';
            arrow.style.visibility = 'visible';
            arrow.style.transition = 'all 0.25s ease-in-out'
        } else {
            arrow.style.opacity = '0';
            arrow.style.visibility = 'hidden';
            arrow.style.transition = 'all 0.25s ease-in-out'
        }
    })
}
catch {
    console.log("to top arrow")
}

//Back to top arrow



//Scroll transition for page sections
try {
    window.addEventListener('scroll', function () {
        const sections = document.querySelectorAll('[id^="page-section-"]');
        const scrollPosition = window.scrollY;

        sections.forEach((section, index) => {
            const start = index * 700;
            const end = start + 700;

            if ($(window).width() > 720) {
                if (scrollPosition >= start && scrollPosition < end) {
                    section.style.opacity = '1';
                    section.style.visibility = 'visible';
                    section.style.transition = 'all 0.5s ease-in-out 0.2s'
                } else {
                    section.style.opacity = '0';
                    section.style.visibility = 'hidden';
                    section.style.transition = 'all 0.3s ease-in-out'
                }
            }
        });
    });
}
catch {
    console.log("scrollfunction pages");
}
//Scroll transition for page sections


//scroll transition for pagination trickers click on sc id
window.addEventListener('wheel', function (event) {
    if (event.deltaY < 0) {
        try {
            document.getElementById('sc-up').click();
        }
        catch {
            console.log("scrollfunction wheel");
        }
    }
    else if (event.deltaY > 0) {
        try {
            document.getElementById('sc-down').click();
        }
        catch {
            console.log("scrollfunction wheel");
        }
    }
});
//scroll transition for pagination trickers click on sc id

