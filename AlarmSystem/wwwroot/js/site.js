$('li button[data-bs-toggle="collapse"]').on('click', function () {
    const $this = $(this);

    $('li button[data-bs-toggle="collapse"]').not($this).each(function () {
        const $btn = $(this);
        const $target = $($btn.attr('data-bs-target'));

        if ($btn.attr('aria-expanded') === 'true') {
            $btn.attr('aria-expanded', 'false');
            $target.collapse('hide');
            $btn.find('.bi-caret-down-fill').removeClass('bi-caret-down-fill').addClass('bi-caret-right-fill');
        }
    });

    const $icon = $this.find('.bi-caret-right-fill, .bi-caret-down-fill');
    if ($this.attr('aria-expanded') === 'true') {
        $icon.removeClass('bi-caret-right-fill').addClass('bi-caret-down-fill');
    } else {
        $icon.removeClass('bi-caret-down-fill').addClass('bi-caret-right-fill');
    }
});
$('#listBtn').on('click', function () {
    const $sidebar = $('#sidebars');
    const $content = $('#content');
    const currentWidth = $sidebar.width();
    if (currentWidth > 0) {
        $sidebar.animate({ width: '0' }, 150, function () {
            $sidebar.css({ 'visibility': 'hidden' });
        });
        $content.animate({ 'margin-left': '0' }, 150);
    } else {
        $sidebar.animate({ width: '280px'}, 150, function () {
            $sidebar.css({ visibility: 'visible' })
        });
        $content.animate({ 'margin-left': '280px' }, 150);
    }
});
const ps = new
    PerfectScrollbar('#container', {
        wheelSpeed: 2,
        wheelPropagation: true,
        minScrollbarLength: 20
    });
ps.update();

(function () {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})();