/**
* Template Name: MyPortfolio
* Template URL: https://bootstrapmade.com/myportfolio-bootstrap-portfolio-website-template/
* Updated: Mar 17 2024 with Bootstrap v5.3.3
* Author: BootstrapMade.com
* License: https://bootstrapmade.com/license/
*/

(function() {
  "use strict";

  /**
   * Easy selector helper function
   */
  const select = (el, all = false) => {
    el = el.trim()
    if (all) {
      return [...document.querySelectorAll(el)]
    } else {
      return document.querySelector(el)
    }
  }

  /**
   * Easy event listener function
   */
  const on = (type, el, listener, all = false) => {
    let selectEl = select(el, all)
    if (selectEl) {
      if (all) {
        selectEl.forEach(e => e.addEventListener(type, listener))
      } else {
        selectEl.addEventListener(type, listener)
      }
    }
  }

  /**
   * Easy on scroll event listener 
   */
  const onscroll = (el, listener) => {
    el.addEventListener('scroll', listener)
  }

    /**
   * ? index post
   */
    const initPopovers = () => {
      const popoverTriggers = select('.question-icon', true);
  
      popoverTriggers.forEach((popoverTrigger) => {
        const popover = new bootstrap.Popover(popoverTrigger, {
          trigger: 'manual',
          html: true,
          title: 'Bu kişi hakkında bilginiz var mı?',
          content: "bu gönderi̇ hakkında bi̇lgi̇ni̇z varsa lütfen <a target='_blank' href='https://www.egm.gov.tr/en-yakin-polis-merkezi'><u>poli̇sle</u></a> i̇leti̇şi̇me geçi̇ni̇z, aksi̇ takdi̇rde bu gönderi̇ni̇n hüküm ve hi̇zmetleri̇ i̇hlal etti̇ği̇ni̇ düşünüyorsaniz lütfen bi̇ze<a href='/Customer/Home/ContactUs'target='_blank'> <u>ulaşınız</u></a>.",
        });
  
        popoverTrigger.addEventListener('mouseenter', () => {
          popover.show();
        });
  
        popoverTrigger.addEventListener('mouseleave', () => {
          setTimeout(() => {
            if (!popoverTrigger.matches(':hover') && !document.querySelector('.popover:hover')) {
              popover.hide();
            }
          }, 200);
        });
      });
    }


    /**
   * ? person's post
   */
    const AciklamaPopovers = () => {
      const popoverTriggers = select('.detayli-aciklama', true);
  
      popoverTriggers.forEach((popoverTrigger) => {
        const popover = new bootstrap.Popover(popoverTrigger, {
          trigger: 'manual',
          html: true,
          title: 'Bu alanında lütfen aşağıdakiler için bilgi veriniz:',
          content: "<li>En son görüldüğü zaman ve yer.</li><li>Kayıp kişinin fiziksel görünümü ve herhangi bir akıl hastalığı olup olmadığı.</li><li>polisin kayıp kişinin hakkındaki son güncellemesi.</li><li>kaybolmasının bir nedeni olduğuna inanıyorsanız.</li>",
        });
  
        popoverTrigger.addEventListener('mouseenter', () => {
          popover.show();
        });
  
        popoverTrigger.addEventListener('mouseleave', () => {
          setTimeout(() => {
            if (!popoverTrigger.matches(':hover') && !document.querySelector('.popover:hover')) {
              popover.hide();
            }
          }, 200);
        });
      });
    }


  /**
   * burgerMenu
   */
  const burgerMenu = select('.burger')
  on('click', '.burger', function(e) {
    burgerMenu.classList.toggle('active');
  })

  /**
   * Porfolio isotope and filter
   */
  window.addEventListener('load', () => {
    let portfolioContainer = select('#portfolio-grid');
    if (portfolioContainer) {
      let portfolioIsotope = new Isotope(portfolioContainer, {
        itemSelector: '.item',
      });

      let portfolioFilters = select('#filters a', true);

      on('click', '#filters a', function(e) {
        e.preventDefault();
        portfolioFilters.forEach(function(el) {
          el.classList.remove('active');
        });
        this.classList.add('active');

        portfolioIsotope.arrange({
          filter: this.getAttribute('data-filter')
        });
        portfolioIsotope.on('arrangeComplete', function() {
          AOS.refresh()
        });
      }, true);
    }   
    initPopovers();
    AciklamaPopovers();

  });

  /**
   * Testimonials slider
   */
  new Swiper('.testimonials-slider', {
    speed: 600,
    loop: true,
    autoplay: {
      delay: 5000,
      disableOnInteraction: false
    },
    slidesPerView: 'auto',
    pagination: {
      el: '.swiper-pagination',
      type: 'bullets',
      clickable: true
    }
  });

  /**
   * Animation on scroll
   */
  window.addEventListener('load', () => {
    AOS.init({
      duration: 1000,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    })
  });

})()