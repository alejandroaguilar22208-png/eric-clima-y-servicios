<?php

require_once __DIR__ . '/../app/config.php';

try {
    $servicios = db()->query('SELECT id, nombre, descripcion, precio_base FROM servicios WHERE activo = 1 ORDER BY id')->fetchAll();
    $equipos   = db()->query('SELECT * FROM equipos_venta WHERE activo = 1 ORDER BY tipo = "split" DESC, frigorias, precio')->fetchAll();
} catch (PDOException $ex) {
    $servicios = [];
    $equipos = [];
}

$viejo  = $_SESSION['form_viejo'] ?? [];
$errorF = $_SESSION['form_error'] ?? '';
unset($_SESSION['form_viejo'], $_SESSION['form_error']);
$enviado = isset($_GET['enviado']);

$wa = fn(string $texto) => 'https://wa.me/' . WHATSAPP . '?text=' . rawurlencode($texto);
$mensajeWa = 'Hola ' . EMPRESA . ', quiero consultar por un aire acondicionado.';

$tipos = ['split' => 'Split', 'ventana' => 'Ventana', 'portatil' => 'Portátil', 'piso_techo' => 'Piso-techo'];

function icono_servicio(string $nombre): string
{
    $n = mb_strtolower($nombre);
    if (str_contains($n, 'desinst')) return 'bi-box-arrow-up-right';
    if (str_contains($n, 'instal'))  return 'bi-tools';
    if (str_contains($n, 'repar'))   return 'bi-wrench-adjustable';
    if (str_contains($n, 'gas'))     return 'bi-thermometer-snow';
    if (str_contains($n, 'manten'))  return 'bi-shield-check';
    return 'bi-snow';
}
?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= e(EMPRESA) ?> · Venta, instalación y reparación de aires acondicionados</title>
    <meta name="description" content="Venta, instalación, reparación, carga de gas y mantenimiento de aires acondicionados en <?= e(ZONA) ?>.">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700;800&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body class="sitio" data-bs-spy="scroll" data-bs-target="#menu" data-bs-offset="90" tabindex="0">

<div class="barra-superior d-none d-md-block">
    <div class="container d-flex justify-content-between">
        <span><i class="bi bi-geo-alt-fill"></i> <?= e(ZONA) ?></span>
        <span>
            <a href="tel:+<?= WHATSAPP ?>"><i class="bi bi-telephone-fill"></i> <?= e(TELEFONO) ?></a>
            <a href="mailto:<?= e(EMAIL) ?>" class="ms-4"><i class="bi bi-envelope-fill"></i> <?= e(EMAIL) ?></a>
            <span class="ms-4"><i class="bi bi-clock-fill"></i> Lun a sáb · 8 a 20 h</span>
        </span>
    </div>
</div>

<nav class="navbar navbar-expand-lg navbar-light bg-white sticky-top menu-principal">
    <div class="container">
        <a class="navbar-brand marca-logo" href="#inicio">
            <span class="logo-icono"><i class="bi bi-snow2"></i></span>
            <span><b>Eric Clima</b><small>y Servicios</small></span>
        </a>
        <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#menu" aria-label="Abrir menú">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="menu">
            <ul class="navbar-nav ms-auto align-items-lg-center gap-lg-1">
                <li class="nav-item"><a class="nav-link" href="#inicio">Inicio</a></li>
                <li class="nav-item"><a class="nav-link" href="#servicios">Servicios</a></li>
                <li class="nav-item"><a class="nav-link" href="#equipos">Equipos en venta</a></li>
                <li class="nav-item"><a class="nav-link" href="#como-trabajamos">Cómo trabajamos</a></li>
                <li class="nav-item"><a class="nav-link" href="#nosotros">Nosotros</a></li>
                <li class="nav-item ms-lg-2 mt-2 mt-lg-0"><a class="btn btn-acento rounded-pill px-4" href="#contacto"><i class="bi bi-clipboard-check"></i> Pedir presupuesto</a></li>
            </ul>
        </div>
    </div>
</nav>

<header class="hero" id="inicio">
    <div class="hero-burbuja b1"></div><div class="hero-burbuja b2"></div>
    <div class="container position-relative">
        <div class="row align-items-center g-5">
            <div class="col-lg-7">
                <span class="etiqueta-hero"><i class="bi bi-geo-alt-fill"></i> Atención en <?= e(ZONA) ?></span>
                <h1 class="hero-titulo">Tu aire acondicionado, <span>bien elegido, bien instalado</span> y funcionando todo el año</h1>
                <p class="hero-texto">Vendemos equipos de las mejores marcas y nos encargamos de todo: instalación, reparación, carga de gas y mantenimiento. Presupuesto sin cargo.</p>
                <div class="d-flex flex-wrap gap-2">
                    <a href="#equipos" class="btn btn-acento btn-lg rounded-pill px-4"><i class="bi bi-snow"></i> Ver equipos</a>
                    <a href="<?= e($wa($mensajeWa)) ?>" class="btn btn-whatsapp btn-lg rounded-pill px-4" target="_blank" rel="noopener"><i class="bi bi-whatsapp"></i> WhatsApp</a>
                </div>
            </div>
            <div class="col-lg-5">
                <div class="row g-3">
                    <?php
                    $ventajas = [
                        ['bi-shield-fill-check', '12 meses', 'de garantía en instalaciones'],
                        ['bi-cash-coin', 'Sin cargo', 'el presupuesto y la visita'],
                        ['bi-award-fill', 'Primeras marcas', 'BGH, Samsung, LG, Surrey y más'],
                        ['bi-bell-fill', 'Te avisamos', 'cuando toca el mantenimiento'],
                    ];
                    foreach ($ventajas as [$ic, $titulo, $texto]): ?>
                        <div class="col-6">
                            <div class="tarjeta-ventaja">
                                <i class="bi <?= $ic ?>"></i>
                                <b><?= $titulo ?></b>
                                <span><?= $texto ?></span>
                            </div>
                        </div>
                    <?php endforeach; ?>
                </div>
            </div>
        </div>
    </div>
</header>

<section class="seccion" id="servicios">
    <div class="container">
        <div class="titulo-seccion">
            <span class="sobretitulo">Lo que hacemos</span>
            <h2>Servicios</h2>
            <p>Precios orientativos. El valor final depende de cada instalación.</p>
        </div>
        <div class="row g-4">
            <?php foreach ($servicios as $s): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="tarjeta-servicio h-100">
                        <div class="icono-servicio"><i class="bi <?= icono_servicio($s['nombre']) ?>"></i></div>
                        <h3><?= e($s['nombre']) ?></h3>
                        <p><?= e($s['descripcion']) ?></p>
                        <div class="d-flex justify-content-between align-items-center mt-auto">
                            <span class="precio-desde">Desde <b><?= pesos($s['precio_base']) ?></b></span>
                            <a href="#contacto" class="enlace-flecha" data-servicio="<?= $s['id'] ?>">Pedir <i class="bi bi-arrow-right"></i></a>
                        </div>
                    </div>
                </div>
            <?php endforeach; ?>
            <?php if (!$servicios): ?>
                <p class="text-center text-muted">Pronto vas a ver acá nuestros servicios.</p>
            <?php endif; ?>
        </div>
    </div>
</section>

<section class="seccion seccion-alt" id="equipos">
    <div class="container">
        <div class="titulo-seccion">
            <span class="sobretitulo">Venta de equipos</span>
            <h2>Equipos en venta</h2>
            <p>Te asesoramos para que compres el equipo justo para tu ambiente. Podés sumar la instalación en el mismo presupuesto.</p>
        </div>

        <div class="calculadora mb-4">
            <div class="row g-3 align-items-center">
                <div class="col-lg-4">
                    <h3 class="h5 mb-1"><i class="bi bi-calculator-fill"></i> ¿Cuántas frigorías necesito?</h3>
                    <p class="small mb-0 opacity-75">Poné los metros cuadrados del ambiente y te marcamos los equipos que te sirven.</p>
                </div>
                <div class="col-sm-4 col-lg-2">
                    <label class="form-label small mb-1" for="m2">Metros²</label>
                    <input type="number" id="m2" class="form-control" min="5" max="200" placeholder="Ej: 15">
                </div>
                <div class="col-sm-4 col-lg-3">
                    <div class="form-check mt-sm-4">
                        <input class="form-check-input" type="checkbox" id="sol">
                        <label class="form-check-label small" for="sol">Le da mucho el sol o tiene mucho vidrio</label>
                    </div>
                </div>
                <div class="col-sm-4 col-lg-3">
                    <div class="resultado-calculo" id="resultado">Necesitás <b>—</b> frigorías</div>
                </div>
            </div>
        </div>

        <div class="filtros mb-4" id="filtros">
            <button class="active" data-filtro="todos">Todos</button>
            <?php foreach ($tipos as $clave => $nombre): ?>
                <button data-filtro="<?= $clave ?>"><?= $nombre ?></button>
            <?php endforeach; ?>
            <button data-filtro="inverter"><i class="bi bi-lightning-charge-fill"></i> Inverter</button>
        </div>

        <div class="row g-4" id="lista-equipos">
            <?php foreach ($equipos as $eq):
                $texto = "Hola, quiero consultar por el equipo {$eq['marca']} {$eq['modelo']} de {$eq['frigorias']} frigorías.";
            ?>
                <div class="col-sm-6 col-lg-4 equipo" data-tipo="<?= e($eq['tipo']) ?>" data-inverter="<?= (int)$eq['inverter'] ?>" data-frigorias="<?= (int)$eq['frigorias'] ?>">
                    <div class="tarjeta-equipo h-100">
                        <div class="equipo-cabecera">
                            <span class="equipo-marca"><?= e($eq['marca']) ?></span>
                            <?php if ((int)$eq['stock'] > 0): ?>
                                <span class="estado-stock disponible"><i class="bi bi-check-circle-fill"></i> En stock</span>
                            <?php else: ?>
                                <span class="estado-stock pedido"><i class="bi bi-clock-history"></i> A pedido</span>
                            <?php endif; ?>
                        </div>
                        <div class="equipo-frigorias"><?= number_format((int)$eq['frigorias'], 0, ',', '.') ?> <small>frigorías</small></div>
                        <h3 class="equipo-modelo"><?= e($eq['modelo']) ?></h3>
                        <div class="equipo-etiquetas">
                            <span><i class="bi bi-wind"></i> <?= e($tipos[$eq['tipo']] ?? $eq['tipo']) ?></span>
                            <span><i class="bi bi-<?= $eq['frio_calor'] ? 'thermometer-half' : 'snow' ?>"></i> <?= $eq['frio_calor'] ? 'Frío / calor' : 'Solo frío' ?></span>
                            <?php if ($eq['inverter']): ?><span class="inverter"><i class="bi bi-lightning-charge-fill"></i> Inverter</span><?php endif; ?>
                        </div>
                        <?php if ($eq['descripcion']): ?><p class="equipo-descripcion"><?= e($eq['descripcion']) ?></p><?php endif; ?>
                        <div class="equipo-pie">
                            <div class="equipo-precio"><?= pesos($eq['precio']) ?></div>
                            <div class="d-flex gap-2">
                                <a href="#contacto" class="btn btn-marca btn-sm rounded-pill px-3 consultar" data-texto="<?= e($texto) ?>">Consultar</a>
                                <a href="<?= e($wa($texto)) ?>" class="btn btn-whatsapp btn-sm rounded-pill" target="_blank" rel="noopener" title="Consultar por WhatsApp"><i class="bi bi-whatsapp"></i></a>
                            </div>
                        </div>
                        <div class="recomendado-cinta"><i class="bi bi-star-fill"></i> Recomendado</div>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
        <p class="text-center text-muted mt-4 mb-0 d-none" id="sin-equipos">No hay equipos de ese tipo por ahora. Escribinos y lo conseguimos.</p>
        <?php if (!$equipos): ?>
            <p class="text-center text-muted">Pronto vas a ver acá los equipos en venta.</p>
        <?php endif; ?>
        <p class="text-center small text-muted mt-4 mb-0">Precios de lista sin instalación. Consultá formas de pago y promociones.</p>
    </div>
</section>

<section class="seccion" id="como-trabajamos">
    <div class="container">
        <div class="titulo-seccion">
            <span class="sobretitulo">Simple y sin vueltas</span>
            <h2>Cómo trabajamos</h2>
        </div>
        <div class="row g-4 pasos">
            <?php
            $pasos = [
                ['bi-chat-dots-fill', 'Nos contactás', 'Por el formulario o por WhatsApp, contanos qué necesitás.'],
                ['bi-clipboard2-check-fill', 'Te pasamos el presupuesto', 'Si hace falta vamos a ver el lugar. El presupuesto es sin cargo.'],
                ['bi-calendar2-week-fill', 'Coordinamos el día', 'Elegimos juntos la fecha y el horario del trabajo.'],
                ['bi-emoji-smile-fill', 'Instalamos y probamos', 'Dejamos el equipo andando, el lugar limpio y la garantía registrada.'],
            ];
            foreach ($pasos as $i => [$ic, $titulo, $texto]): ?>
                <div class="col-sm-6 col-lg-3">
                    <div class="paso">
                        <div class="paso-icono"><i class="bi <?= $ic ?>"></i><span><?= $i + 1 ?></span></div>
                        <h3><?= $titulo ?></h3>
                        <p><?= $texto ?></p>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<section class="seccion seccion-oscura" id="nosotros">
    <div class="container">
        <div class="row g-5 align-items-center">
            <div class="col-lg-6">
                <span class="sobretitulo claro">Quiénes somos</span>
                <h2 class="fw-bold mb-4">Una empresa familiar de <?= e(explode(' y ', ZONA)[0]) ?></h2>
                <p><b><?= e(EMPRESA) ?></b> nació del oficio de Eric, técnico en refrigeración, que empezó instalando equipos para vecinos y conocidos y fue sumando clientes gracias al boca en boca.</p>
                <p>Hoy atendemos casas, comercios y oficinas de la ciudad y la zona. Nos encargamos del trabajo de punta a punta: te ayudamos a elegir el equipo, lo instalamos con materiales de primera y después seguimos en contacto para el mantenimiento.</p>
                <p class="mb-0">Nuestra forma de trabajar es simple: <b>cumplir con el día y la hora</b>, explicar todo claro y dejar el lugar como lo encontramos.</p>
            </div>
            <div class="col-lg-6">
                <div class="row g-3">
                    <?php
                    $valores = [
                        ['bi-clock-fill', 'Puntualidad', 'Llegamos el día y a la hora que acordamos.'],
                        ['bi-hand-thumbs-up-fill', 'Trabajo prolijo', 'Cañerías ocultas, soportes firmes y todo limpio.'],
                        ['bi-chat-heart-fill', 'Trato directo', 'Hablás siempre con quien hace el trabajo.'],
                        ['bi-patch-check-fill', 'Garantía real', '12 meses sobre la mano de obra, por escrito.'],
                    ];
                    foreach ($valores as [$ic, $titulo, $texto]): ?>
                        <div class="col-sm-6">
                            <div class="tarjeta-valor">
                                <i class="bi <?= $ic ?>"></i>
                                <h3><?= $titulo ?></h3>
                                <p><?= $texto ?></p>
                            </div>
                        </div>
                    <?php endforeach; ?>
                </div>
            </div>
        </div>
    </div>
</section>

<section class="seccion" id="contacto">
    <div class="container">
        <div class="titulo-seccion">
            <span class="sobretitulo">Escribinos</span>
            <h2>Pedí tu presupuesto</h2>
            <p>Completá el formulario y te contactamos a la brevedad. Es sin cargo y sin compromiso.</p>
        </div>
        <div class="row g-4">
            <div class="col-lg-4">
                <div class="tarjeta-contacto h-100">
                    <h3>Datos de contacto</h3>
                    <a class="dato" href="tel:+<?= WHATSAPP ?>"><i class="bi bi-telephone-fill"></i><span><small>Teléfono</small><?= e(TELEFONO) ?></span></a>
                    <a class="dato" href="mailto:<?= e(EMAIL) ?>"><i class="bi bi-envelope-fill"></i><span><small>Email</small><?= e(EMAIL) ?></span></a>
                    <div class="dato"><i class="bi bi-geo-alt-fill"></i><span><small>Zona</small><?= e(ZONA) ?></span></div>
                    <div class="dato"><i class="bi bi-clock-fill"></i><span><small>Horario</small>Lunes a sábado de 8 a 20 h</span></div>
                    <a href="<?= e($wa($mensajeWa)) ?>" class="btn btn-whatsapp btn-lg w-100 rounded-pill mt-3" target="_blank" rel="noopener"><i class="bi bi-whatsapp"></i> Escribir por WhatsApp</a>
                </div>
            </div>
            <div class="col-lg-8">
                <?php if ($enviado): ?>
                    <div class="alert alert-success"><i class="bi bi-check-circle-fill"></i> ¡Gracias! Recibimos tu pedido y te vamos a contactar pronto.</div>
                <?php endif; ?>
                <?php if ($errorF): ?>
                    <div class="alert alert-warning"><?= e($errorF) ?></div>
                <?php endif; ?>
                <form method="post" action="solicitud.php" class="tarjeta-formulario" id="form-contacto" novalidate>
                    <?= csrf_campo() ?>
                    <input type="text" name="sitio_web" class="d-none" tabindex="-1" autocomplete="off">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label" for="nombre">Nombre y apellido *</label>
                            <input class="form-control" id="nombre" name="nombre" maxlength="120" required value="<?= e($viejo['nombre'] ?? '') ?>">
                            <div class="invalid-feedback">Escribí tu nombre y apellido.</div>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="telefono">Teléfono *</label>
                            <input class="form-control" id="telefono" name="telefono" maxlength="30" required value="<?= e($viejo['telefono'] ?? '') ?>">
                            <div class="invalid-feedback">Escribí un teléfono válido.</div>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="direccion">Dirección</label>
                            <input class="form-control" id="direccion" name="direccion" maxlength="150" value="<?= e($viejo['direccion'] ?? '') ?>">
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="servicio">Servicio</label>
                            <select class="form-select" id="servicio" name="servicio_id">
                                <option value="">Compra de equipo / otro</option>
                                <?php foreach ($servicios as $s): ?>
                                    <option value="<?= $s['id'] ?>" <?= ($viejo['servicio_id'] ?? '') == $s['id'] ? 'selected' : '' ?>><?= e($s['nombre']) ?></option>
                                <?php endforeach; ?>
                            </select>
                        </div>
                        <div class="col-12">
                            <label class="form-label" for="mensaje">Contanos qué necesitás</label>
                            <textarea class="form-control" id="mensaje" name="mensaje" rows="4" maxlength="1000" placeholder="Ej: instalar un split de 3000 frigorías en un dormitorio de planta alta"><?= e($viejo['mensaje'] ?? '') ?></textarea>
                        </div>
                        <div class="col-12">
                            <button class="btn btn-acento btn-lg w-100 rounded-pill" type="submit"><i class="bi bi-send-fill"></i> Enviar pedido</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</section>

<footer class="pie">
    <div class="container">
        <div class="row g-4">
            <div class="col-md-5">
                <div class="marca-logo claro mb-2"><span class="logo-icono"><i class="bi bi-snow2"></i></span><span><b>Eric Clima</b><small>y Servicios</small></span></div>
                <p class="mb-0">Venta, instalación, reparación y mantenimiento de aires acondicionados en <?= e(ZONA) ?>.</p>
            </div>
            <div class="col-6 col-md-3">
                <h4>Secciones</h4>
                <a href="#servicios">Servicios</a><a href="#equipos">Equipos en venta</a><a href="#nosotros">Nosotros</a><a href="#contacto">Contacto</a>
            </div>
            <div class="col-6 col-md-4">
                <h4>Contacto</h4>
                <a href="tel:+<?= WHATSAPP ?>"><i class="bi bi-telephone"></i> <?= e(TELEFONO) ?></a>
                <a href="mailto:<?= e(EMAIL) ?>"><i class="bi bi-envelope"></i> <?= e(EMAIL) ?></a>
                <a href="<?= e($wa($mensajeWa)) ?>" target="_blank" rel="noopener"><i class="bi bi-whatsapp"></i> WhatsApp</a>
            </div>
        </div>
        <div class="pie-final">
            <span>© <?= date('Y') ?> <?= e(EMPRESA) ?></span>
            <a href="../admin/login.php"><i class="bi bi-lock-fill"></i> Acceso al panel</a>
        </div>
    </div>
</footer>

<a href="<?= e($wa($mensajeWa)) ?>" class="boton-whatsapp" target="_blank" rel="noopener" aria-label="Escribir por WhatsApp"><i class="bi bi-whatsapp"></i></a>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script>
const form = document.getElementById('form-contacto');
form.addEventListener('submit', ev => {
    const nombreMal = form.nombre.value.trim().length < 3;
    const telMal = !/^[0-9 +()-]{6,}$/.test(form.telefono.value.trim());
    form.nombre.classList.toggle('is-invalid', nombreMal);
    form.telefono.classList.toggle('is-invalid', telMal);
    if (nombreMal || telMal) ev.preventDefault();
});

document.querySelectorAll('.consultar').forEach(b => b.addEventListener('click', () => {
    form.servicio_id.value = '';
    form.mensaje.value = b.dataset.texto;
    setTimeout(() => form.nombre.focus(), 600);
}));
document.querySelectorAll('[data-servicio]').forEach(b => b.addEventListener('click', () => {
    form.servicio_id.value = b.dataset.servicio;
}));

const equipos = [...document.querySelectorAll('.equipo')];
const sinEquipos = document.getElementById('sin-equipos');
let filtro = 'todos';

function aplicarFiltro() {
    let visibles = 0;
    equipos.forEach(eq => {
        const ok = filtro === 'todos' || eq.dataset.tipo === filtro || (filtro === 'inverter' && eq.dataset.inverter === '1');
        eq.classList.toggle('d-none', !ok);
        if (ok) visibles++;
    });
    sinEquipos.classList.toggle('d-none', visibles > 0 || !equipos.length);
}

document.querySelectorAll('#filtros button').forEach(b => b.addEventListener('click', () => {
    document.querySelectorAll('#filtros button').forEach(x => x.classList.remove('active'));
    b.classList.add('active');
    filtro = b.dataset.filtro;
    aplicarFiltro();
}));

const m2 = document.getElementById('m2');
const sol = document.getElementById('sol');
const resultado = document.getElementById('resultado');

function calcular() {
    const metros = parseFloat(m2.value);
    equipos.forEach(eq => eq.classList.remove('recomendado'));
    if (!metros || metros <= 0) { resultado.innerHTML = 'Necesitás <b>—</b> frigorías'; return; }
    let frig = metros * 2.6 * 50 * (sol.checked ? 1.2 : 1);
    frig = Math.max(2250, Math.ceil(frig / 50) * 50);
    resultado.innerHTML = 'Necesitás unas <b>' + frig.toLocaleString('es-AR') + '</b> frigorías';
    const candidatos = equipos.filter(eq => +eq.dataset.frigorias >= frig);
    if (!candidatos.length) return;
    const minimo = Math.min(...candidatos.map(eq => +eq.dataset.frigorias));
    candidatos.filter(eq => +eq.dataset.frigorias <= minimo * 1.25).forEach(eq => eq.classList.add('recomendado'));
}
m2.addEventListener('input', calcular);
sol.addEventListener('change', calcular);
</script>
</body>
</html>
