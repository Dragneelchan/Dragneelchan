"use strict";

function initDashboard() {
    var global = {
        tooltipOptions: {
            placement: "right",
        },
        menuClass: ".c-menu",
    };

    var menuChangeActive = function menuChangeActive(el) {
        var hasSubmenu = $(el).hasClass("has-submenu");
        $(global.menuClass + " .is-active").removeClass("is-active");
        $(el).addClass("is-active");
    };

    var sidebarChangeWidth = function sidebarChangeWidth() {
        var $menuItemsTitle = $("li .menu-item__title");

        $("body").toggleClass("sidebar-is-reduced sidebar-is-expanded");
        $(".hamburger-toggle").toggleClass("is-opened");

        if ($("body").hasClass("sidebar-is-expanded")) {
            $('[data-toggle="tooltip"]').tooltip("destroy");
        } else {
            $('[data-toggle="tooltip"]').tooltip(global.tooltipOptions);
        }
    };

    return {
        init: function init() {
            $(".js-hamburger").on("click", sidebarChangeWidth);

            $(".js-menu li").on("click", function (e) {
                menuChangeActive(e.currentTarget);
            });

            $('[data-toggle="tooltip"]').tooltip(global.tooltipOptions);
        },
    };
}

// Initialize Dashboard
initDashboard().init();

let isDragging = false;
let offsetX, offsetY;
let draggedElement;

function startDrag(e) {
    isDragging = true;
    draggedElement = e.target.parentElement;
    offsetX = e.clientX - draggedElement.getBoundingClientRect().left;
    offsetY = e.clientY - draggedElement.getBoundingClientRect().top;
}

function moveElement(e) {
    if (isDragging && draggedElement) {
        draggedElement.style.left = e.clientX - offsetX + 'px';
        draggedElement.style.top = e.clientY - offsetY + 'px';
    }
}

function stopDragging() {
    isDragging = false;
}

function toggleDropdown() {
    const dropdown = document.getElementById('dropdown');
    dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
}

// Add event listeners to start and stop dragging
document.addEventListener('mousedown', function (e) {
    if (e.target.classList.contains('draggable-header')) {
        startDrag(e);
        document.addEventListener('mousemove', moveElement);
    }
});

document.addEventListener('mouseup', stopDragging);
