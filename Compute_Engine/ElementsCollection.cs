using Compute_Engine.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    [Serializable]
    public class ElementsCollection: IEnumerable<ElementsBase>
    {
        private List<ElementsBase> _children;
        internal ElementsBase _parent;

        /// <summary>Zwróć element o podanym indeksie.</summary>
        /// <param name="index">Numer elementu.</param>
        public ElementsBase this[int index]
        {
            get
            {
                return _children[index];
            }
        }

        public ElementsCollection()
        {
            _children = new List<ElementsBase>();
        }

        public ElementsCollection(ElementsBase element)
        {
            ElementsBase loc = (ElementsBase)element.Clone();
            loc.Parent = _parent;
            _children = new List<ElementsBase>() { loc };
        }

        public ElementsCollection(params ElementsBase[] element)
        {
            _children = new List<ElementsBase>();
            ElementsBase loc = null;

            foreach (ElementsBase eb in element)
            {
                loc = (ElementsBase)eb.Clone();
                loc.Parent = _parent;
                _children.Add(loc);
            }
        }

        /// <summary>Dodaj nowy element.</summary>
        /// <param name="element">Element do dodania.</param>
        public void Add(ElementsBase element)
        {
            ElementsBase loc = (ElementsBase)element.Clone();
            loc.Parent = _parent;
            _children.Add(loc);
        }

        /// <summary>Dodaj nową grupę elementów.</summary>
        /// <param name="element">Tablica elementów do dodania.</param>
        public void AddRange(params ElementsBase[] element)
        {
            ElementsBase loc = null;

            foreach (ElementsBase eb in element)
            {
                loc = (ElementsBase)eb.Clone();
                loc.Parent = _parent;
                _children.Add(loc);
            }
        }

        /// <summary>Usuń element.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public void RemoveByName(string elementName)
        {
            ElementsBase element = Find(elementName);
            _children.Remove(element);
        }

        /// <summary>Usuń element o podanym indeksie.</summary>
        /// <param name="index">Numer elementu.</param>
        public void RemoveAt(int index)
        {
            _children.RemoveAt(index);
        }

        /// <summary>Oblicz poziom ciśnienia akustycznego.</summary>
        public Dictionary<Room, double[]> ComputeSoundPressureLevel()
        {
            double[] loc_result = new double[8] { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            Dictionary<Room, double[]> overall_result = new Dictionary<Room, double[]>();
            List<List<ElementsBase>> local = ElementsLists();

            foreach (List<ElementsBase> list in local)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    if (list[i-1].Parent == list[i])
                    {
                        if (list[i] is Junction)
                        {
                            loc_result = MathOperation.OctaveSubstract(loc_result, ((Junction)list[i]).Branch.Attenuation());
                            loc_result = MathOperation.OctaveDecibelAdd(loc_result, ((Junction)list[i]).Branch.Noise());
                        }
                        else if (list[i] is DoubleJunction)
                        {
                            if (((DoubleJunction)list[i]).BranchRight.Elements.Contains(list[i - 1]))
                            {
                                loc_result = MathOperation.OctaveSubstract(loc_result, ((DoubleJunction)list[i]).BranchRight.Attenuation());
                                loc_result = MathOperation.OctaveDecibelAdd(loc_result, ((DoubleJunction)list[i]).BranchRight.Noise());
                            }
                            else
                            {
                                loc_result = MathOperation.OctaveSubstract(loc_result, ((DoubleJunction)list[i]).BranchLeft.Attenuation());
                                loc_result = MathOperation.OctaveDecibelAdd(loc_result, ((DoubleJunction)list[i]).BranchLeft.Noise());
                            }
                        }
                        else if (list[i] is TJunction)
                        {
                            if (((TJunction)list[i]).BranchRight.Elements.Contains(list[i - 1]))
                            {
                                loc_result = MathOperation.OctaveSubstract(loc_result, ((TJunction)list[i]).BranchRight.Attenuation());
                                loc_result = MathOperation.OctaveDecibelAdd(loc_result, ((TJunction)list[i]).BranchRight.Noise());
                            }
                            else
                            {
                                loc_result = MathOperation.OctaveSubstract(loc_result, ((TJunction)list[i]).BranchLeft.Attenuation());
                                loc_result = MathOperation.OctaveDecibelAdd(loc_result, ((TJunction)list[i]).BranchLeft.Noise());
                            }
                        }
                    }
                    else
                    {
                        loc_result = MathOperation.OctaveSubstract(loc_result, list[i].Attenuation());
                        loc_result = MathOperation.OctaveDecibelAdd(loc_result, list[i].Noise());
                    }
                }

                loc_result = MathOperation.OctaveSubstract(loc_result, list[0].Attenuation());
                loc_result = MathOperation.OctaveDecibelAdd(loc_result, list[0].Noise());
                overall_result.Add((Room)list[0], loc_result);
            }
            return overall_result;
        }

        /// <summary>Znajdź element o podanej nazwie.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public ElementsBase Find(string elementName)
        {
            ElementsBase result = null;
            ElementsBase loc = null;

            foreach (ElementsBase element in _children)
            {
                if (element.Name == elementName)
                {
                    result = element;
                    break;
                }
                else if (element.Type == ElementType.Junction)
                {
                    loc = ((Junction)element).Branch.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                }
                else if (element.Type == ElementType.DoubleJunction)
                {
                    loc = ((DoubleJunction)element).BranchRight.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((DoubleJunction)element).BranchLeft.Elements.Find(elementName);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
                else if (element.Type == ElementType.TJunction)
                {
                    loc = ((TJunction)element).BranchRight.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((TJunction)element).BranchLeft.Elements.Find(elementName);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>Zwróć ciąg elementów do obiektu o podanej nazwie.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public List<ElementsBase> ElementsRow(string elementName)
        {
            ElementsBase _temp = Find(elementName);
            List<ElementsBase> _loc = new List<ElementsBase>();
            ElementsBase _dtee = _temp;
            int _index = 0;

            if (_temp.Parent != null)
            {
                do
                {
                    #region ElementType
                    if (_temp.Parent is Junction)
                    {
                        Check(((Junction)_temp.Parent).Branch.Elements);
                        if (_dtee != null)
                        {
                            for (int j = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; j >= 0; j--)
                            {
                                if (_dtee == ((Junction)_temp.Parent).Branch.Elements[j]) { _index = j; break; }
                            }
                        }
                        else
                        {
                            if (((Junction)_temp.Parent).Branch.Elements.Last().Type == ElementType.Room) { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 2; }
                            else { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; }
                        }

                        for (int j = _index; j >= 0; j--)
                        {
                            _loc.Add(((Junction)_temp.Parent).Branch.Elements[j]);
                        }

                        _dtee = null;
                    }
                    else if (_temp.Parent is DoubleJunction)
                    {
                        if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                        {
                            Check(((DoubleJunction)_temp.Parent).BranchRight.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((DoubleJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                else { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((DoubleJunction)_temp.Parent).BranchRight.Elements[j]);
                            }
                        }
                        else
                        {
                            Check(((DoubleJunction)_temp.Parent).BranchLeft.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((DoubleJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                else { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]);
                            }
                        }

                        _dtee = _temp.Parent;
                    }
                    else if (_temp.Parent is TJunction)
                    {
                        if (((TJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                        {
                            Check(((TJunction)_temp.Parent).BranchRight.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((TJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((TJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                else { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((TJunction)_temp.Parent).BranchRight.Elements[j]);
                            }
                        }
                        else
                        {
                            Check(((TJunction)_temp.Parent).BranchLeft.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((TJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((TJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                else { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((TJunction)_temp.Parent).BranchLeft.Elements[j]);
                            }
                        }

                        _dtee = null;
                    }
                    #endregion

                    _temp = _temp.Parent;
                } while (_temp.Parent != null);

                Check(this);
                for (int j = _children.Count - 1; j >= 0; j--)
                {
                    if (_temp == _children[j]) { _index = j; break; }
                }

                for (int j = _index; j >= 0; j--)
                {
                    _loc.Add(_children[j]);
                }
            }
            else
            {
                Check(this);
                for (int j = _children.Count - 1; j >= 0; j--)
                {
                    if (_temp == _children[j]) { _index = j; break; }
                }

                for (int j = _index; j >= 0; j--)
                {
                    _loc.Add(_children[j]);
                }
            }

            _loc.Reverse();
            return _loc;
        }

        private ElementsBase FindElementType(List<ElementsBase> Exist)
        {
            ElementsBase result = null;
            ElementsBase loc = null;

            foreach (ElementsBase element in _children)
            {
                if (element.Type == ElementType.Room && Exist.Contains(element) == false)
                {
                    result = element;
                    break;
                }
                else if (element.Type == ElementType.Junction)
                {
                    loc = ((Junction)element).Branch.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                }
                else if (element.Type == ElementType.DoubleJunction)
                {
                    loc = ((DoubleJunction)element).BranchRight.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((DoubleJunction)element).BranchLeft.Elements.FindElementType(Exist);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
                else if (element.Type == ElementType.TJunction)
                {
                    loc = ((TJunction)element).BranchRight.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((TJunction)element).BranchLeft.Elements.FindElementType(Exist);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private List<List<ElementsBase>> ElementsLists()
        {
            List<ElementsBase> _count = new List<ElementsBase>();

            do
            {
                _count.Add(FindElementType(_count));
            } while (FindElementType(_count) != null);
            
            List<List<ElementsBase>> _result = new List<List<ElementsBase>>();

            for (int i = 0; i < _count.Count; i++)
            {
                ElementsBase _temp = _count[i];
                List<ElementsBase> _loc = new List<ElementsBase>() { _count[i] };
                ElementsBase _dtee = null;

                if (_temp.Parent != null)
                {
                    int _index = 0;

                    do
                    {
                        #region ElementType
                        if (_temp.Parent is Junction)
                        {
                            Check(((Junction)_temp.Parent).Branch.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((Junction)_temp.Parent).Branch.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((Junction)_temp.Parent).Branch.Elements.Last().Type == ElementType.Room) { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 2; }
                                else { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((Junction)_temp.Parent).Branch.Elements[j]);
                            }

                            _dtee = null;
                        }
                        else if (_temp.Parent is DoubleJunction)
                        {
                            if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                            {
                                Check(((DoubleJunction)_temp.Parent).BranchRight.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((DoubleJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                    else { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((DoubleJunction)_temp.Parent).BranchRight.Elements[j]);
                                }
                            }
                            else
                            {
                                Check(((DoubleJunction)_temp.Parent).BranchLeft.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((DoubleJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                    else { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]);
                                }
                            }

                            _dtee = _temp.Parent;
                        }
                        else if (_temp.Parent is TJunction)
                        {
                            if (((TJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                            {
                                Check(((TJunction)_temp.Parent).BranchRight.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((TJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((TJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                    else { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((TJunction)_temp.Parent).BranchRight.Elements[j]);
                                }
                            }
                            else
                            {
                                Check(((TJunction)_temp.Parent).BranchLeft.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((TJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((TJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                    else { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((TJunction)_temp.Parent).BranchLeft.Elements[j]);
                                }
                            }

                            _dtee = null;
                        }
                        #endregion

                        _temp = _temp.Parent;
                    } while (_temp.Parent != null);

                    Check(this);
                    for (int j = _children.Count - 1; j >= 0; j--)
                    {
                        if (_temp == _children[j]) { _index = j; break; }
                    }

                    for (int j = _index; j >= 0; j--)
                    {
                        _loc.Add(_children[j]);
                    }
                }
                else
                {
                    Check(this);
                    for (int j = _children.Count - 2; j >= 0; j-- )
                    {
                        _loc.Add(_children[j]);
                    }
                }

                _result.Add(_loc);
            }
            return _result;
        }

        private void Check(ElementsCollection elementsCollection)
        {
            if (elementsCollection == null) { throw new ArgumentNullException(); }
            else if (elementsCollection.Count() == 0) { throw new Exception("Brak wystarczającej ilości elementów do przeprowadzenia obliczeń."); }
            else if ((from element in elementsCollection where element.Type == ElementType.Room select element).ToList().Count > 1)
            { throw new Exception("Zbyt duża liczba elementów typu Room w sekwencji."); }
            else if ((from element in elementsCollection where element.Type == ElementType.TJunction select element).ToList().Count > 1)
            { throw new Exception("Zbyt duża liczba elementów typu T-trónik w sekwencji."); }
            else if (elementsCollection.Last().Type != ElementType.Room && elementsCollection.Last().Type != ElementType.TJunction)
            { throw new Exception("Nieprawidłowa kolejność elementów w sekwencji."); }
            else if ((from element in elementsCollection where element.Type == ElementType.TJunction select element).ToList().Count == 1 &&
                (from element in elementsCollection where element.Type == ElementType.Room select element).ToList().Count == 1)
            { throw new Exception("Elementy typu T-trónik i Room nie mogą występować w tym samym ciągu."); }
        }

        public IEnumerator<ElementsBase> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
