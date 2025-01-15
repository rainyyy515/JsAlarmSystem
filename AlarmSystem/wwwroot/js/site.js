﻿$('li button[data-bs-toggle="collapse"]').on('click', function () {
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
    const currentWidth = $sidebar.width();
    if (currentWidth > 0) {
        $sidebar.animate({ width: '0','opacity':'0' }, 400);
    } else {
        $sidebar.animate({ width: '280px', 'opacity':'1' }, 400);
    }
});
const ps = new
    PerfectScrollbar('#container', {
        wheelSpeed: 2,
        wheelPropagation: true,
        minScrollbarLength: 20
    });
ps.update();