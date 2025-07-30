$(document).ready(function () {

    function carregarItensPedido(numPed) {
        $.ajax({
            url: '/Home/GetItensPedido',
            type: 'GET',
            data: { numPed: numPed },
            success: function (data) {
                var tbody = $('#itensPedidoTable tbody');
                tbody.empty();

                if (data.length === 0) {
                    tbody.append('<tr><td colspan="5">Nenhum item encontrado.</td></tr>');
                } else {
                    $.each(data, function (index, item) {
                        tbody.append('<tr>' +
                            '<td>' + item.CODPROD + '</td>' +
                            '<td>' + item.DESCRICAO + '</td>' +
                            '<td>' + item.QT + '</td>' +
                            '<td>' + item.PVENDA + '</td>' +
                            '<td>' + item.SUBTOTAL + '</td>' +
                            '</tr>');
                    });
                }
                $('#modalNumPed').text(numPed);
                $('#itemPedidoModal').modal('show');
            },
            error: function () {
                alert('Erro ao carregar os itens do pedido.');
            }
        });
    }

    $('#filterButton').click(function () {
        var filtro = $('#numpedFilter').val();
        window.location.href = '/Home/Index?numPedFilter=' + encodeURIComponent(filtro);
    });

    $('#calculateButton').click(function () {
        var valorFrete = parseFloat($('#valorFrete').val());
        if (isNaN(valorFrete)) {
            alert('Informe um valor válido para o frete.');
            return;
        }

        var totalVlAtend = parseFloat($('#totalVlAtend').text().replace('.', '').replace(',', '.'));
        if (isNaN(totalVlAtend) || totalVlAtend === 0) {
            alert('Total VL.ATEND inválido.');
            return;
        }

        var porcentagemFrete = (valorFrete / totalVlAtend) * 100;

        $('#porcentagemFrete').text(porcentagemFrete.toFixed(2));
    });

    window.carregarDados = function (rota) {
        // Se você quiser implementar filtro por rota, faça aqui.
        alert('Implementação do filtro por rota ainda não disponível: ' + rota);
    };

    // Carregar modal ao clicar numa linha da tabela (exemplo):
    $('.data-grid tbody tr').on('click', function () {
        var numPed = $(this).data('numped');
        if (numPed) {
            carregarItensPedido(numPed);
        }
    });

});
