document.querySelectorAll('.faq-q').forEach(function (b) {
    b.addEventListener('click', function () {
        b.parentElement.classList.toggle('open');
    });
});

var faqSearch = document.getElementById('faq-search');
if (faqSearch) {
    faqSearch.addEventListener('input', function () {
        var v = faqSearch.value.toLowerCase();
        document.querySelectorAll('.faq-item').forEach(function (i) {
            i.style.display = i.textContent.toLowerCase().indexOf(v) >= 0 ? '' : 'none';
        });
    });
}

var card = document.getElementById('CardNumber');
if (card) {
    card.addEventListener('input', function () {
        var d = card.value.replace(/\D/g, '').slice(0, 16);
        card.value = d.replace(/(.{4})/g, '$1 ').trim();
    });
}

var exp = document.getElementById('Expiry');
if (exp) {
    exp.addEventListener('input', function () {
        var d = exp.value.replace(/\D/g, '').slice(0, 4);
        exp.value = d.length > 2 ? d.slice(0, 2) + '/' + d.slice(2) : d;
    });
}

var fields = document.getElementById('card-fields');
document.querySelectorAll('input[name="Method"]').forEach(function (r) {
    r.addEventListener('change', function () {
        if (fields) fields.style.display = r.value === 'card' ? '' : 'none';
    });
});
var checked = document.querySelector('input[name="Method"]:checked');
if (fields && checked && checked.value !== 'card') fields.style.display = 'none';
