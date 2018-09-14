using System;
using System.Linq;
using System.Collections.Generic;
using CoreEscuela.Entidades;

namespace CoreEscuela.App
{
    public class Reporteador
    {
        Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> _diccionario;
        public Reporteador(Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> dicObsEsc)
        {
            if (dicObsEsc == null)
                throw new ArgumentNullException(nameof(dicObsEsc));

            _diccionario = dicObsEsc;
        }

        public IEnumerable<Evaluación> GetListaEvaluaciones()
        {
            if (_diccionario.TryGetValue(LlaveDiccionario.Evaluación,
                                                 out IEnumerable<ObjetoEscuelaBase> lista))
            {
                return lista.Cast<Evaluación>();
            }
            {
                return new List<Evaluación>();
            }
        }

        public IEnumerable<string> GetListaAsignaturas()
        {
            return GetListaAsignaturas(
                    out var dummy);
        }

        public IEnumerable<string> GetListaAsignaturas(
            out IEnumerable<Evaluación> listaEvaluaciones)
        {
            listaEvaluaciones = GetListaEvaluaciones();

            return (from Evaluación ev in listaEvaluaciones
                    select ev.Asignatura.Nombre).Distinct(); ;
        }

        public Dictionary<string, IEnumerable<Evaluación>> GetDicEvaluaXAsig()
        {
            var dictaRta = new Dictionary<string, IEnumerable<Evaluación>>();

            var listaAsig = GetListaAsignaturas(out var listaEval);

            foreach (var asig in listaAsig)
            {
                var evalsAsig = from eval in listaEval
                                where eval.Asignatura.Nombre == asig
                                select eval;

                dictaRta.Add(asig, evalsAsig);
            }

            return dictaRta;
        }

        public Dictionary<string, IEnumerable<object>> GetPromeAlumnPorAsignatura()
        {
            var rta = new Dictionary<string, IEnumerable<object>>();
            var dicEvalXAsig = GetDicEvaluaXAsig();

            foreach (var asigConEval in dicEvalXAsig)
            {
                var promsAlumn = from eval in asigConEval.Value
                            group eval by new{
                                 eval.Alumno.UniqueId,
                                 eval.Alumno.Nombre}
                            into grupoEvalsAlumno
                            select new AlumnoPromedio
                            { 
                                alumnoid = grupoEvalsAlumno.Key.UniqueId,
                                alumnoNombre = grupoEvalsAlumno.Key.Nombre,
                                promedio = grupoEvalsAlumno.Average(evaluacion => evaluacion.Nota)
                            };
                 
                 rta.Add(asigConEval.Key, promsAlumn);           
            }

            return rta;
        }
}
}